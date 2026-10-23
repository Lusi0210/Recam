using System.Text;
using Azure.Storage.Blobs;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Remp.API.Middlewares;
using Remp.Common.Responses;
using Remp.Common.Settings;
using Remp.DataAccess.Collections;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Common;
using Remp.Repository.Interfaces;
using Remp.Repository.Repositories;
using Remp.Service.Interfaces;
using Remp.Service.Mappers;
using Remp.Service.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<RempDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("RempDb")));

builder.Services.AddIdentity<User, IdentityRole>()
       .AddEntityFrameworkStores<RempDbContext>()
       .AddDefaultTokenProviders();


JwtSettings jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = async (JwtBearerChallengeContext context) =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            ApiResponse<object> response = ApiResponse<object>.FailureResponse("Unauthorized. Please log in.");
            await context.Response.WriteAsJsonAsync(response);
        },
        OnForbidden = async (ForbiddenContext context) =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            ApiResponse<object> response = ApiResponse<object>.FailureResponse("Forbidden. You do not have permission to access this resource.");
            await context.Response.WriteAsJsonAsync(response);
        }
    };
});

builder.Services.AddAuthorization();


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
builder.Services.AddValidatorsFromAssembly(typeof(MappingProfile).Assembly);


builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Host.UseSerilog();

builder.Services.Configure<AzureBlobSettings>(builder.Configuration.GetSection("AzureBlobSettings"));
AzureBlobSettings blobSettings = builder.Configuration.GetSection("AzureBlobSettings").Get<AzureBlobSettings>()!;

builder.Services.Configure<FrontendSettings>(builder.Configuration.GetSection("FrontendSettings"));

FrontendSettings frontendSettings = builder.Configuration.GetSection("FrontendSettings").Get<FrontendSettings>()!;

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(frontendSettings.BaseUrl)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .WithExposedHeaders("Content-Disposition");
    });
});

builder.Services.AddSingleton<BlobServiceClient>(new BlobServiceClient(blobSettings.ConnectionString));
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<MongoDbContext>();

builder.Services.AddScoped<IUserActivityLogRepository, UserActivityLogRepository>();

builder.Services.AddScoped<IAgentRepository, AgentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IListingCaseRepository, ListingCaseRepository>();
builder.Services.AddScoped<IListingCaseHistoryRepository, ListingCaseHistoryRepository>();
builder.Services.AddScoped<IListingCaseService, ListingCaseService>();

builder.Services.AddScoped<IPhotographyCompanyRepository,PhotographyCompanyRepository>();
builder.Services.AddScoped<IAgentService,AgentService>();

builder.Services.AddScoped<IEmailService, DevEmailService>();
builder.Services.AddScoped<IPhotographyCompanyActivityLogRepository,PhotographyCompanyActivityLogRepository>();

builder.Services.AddScoped<ICaseContactRepository,CaseContactRepository>();
builder.Services.AddScoped<ICaseContactService,CaseContactService>();

builder.Services.AddScoped<IMediaAssetRepository, MediaAssetRepository>();
builder.Services.AddScoped<IMediaAssetService, MediaAssetService>();

builder.Services.AddScoped<IActivityLogService, ActivityLogService>();

var app = builder.Build();

await DbSeeder.SeedAsync(app.Services, app.Environment.IsDevelopment());

BlobServiceClient blobServiceClient = app.Services.GetRequiredService<BlobServiceClient>();
await blobServiceClient
    .GetBlobContainerClient(blobSettings.ContainerName)
    .CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.None);

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>(); 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();


