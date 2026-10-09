using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Remp.API.Middlewares;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
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


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

await DbSeeder.SeedAsync(app.Services, app.Environment.IsDevelopment());

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>(); 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();


app.Run();


