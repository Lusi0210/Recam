using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Remp.Models.Entities; 

namespace Remp.DataAccess.Data;

public class RempDbContext : IdentityDbContext<ApplicationUser>
{
    public RempDbContext(DbContextOptions<RempDbContext> options) : base(options)
    {
        
    }

    public DbSet<ListingCase> ListingCases {get;set;}
    public DbSet<Agent> Agents {get;set;}
    public DbSet<AgentListingCase> AgentListingCases {get;set;}
    public DbSet<AgentPhotographyCompany> AgentPhotographyCompanies {get;set;}
    public DbSet<CaseContact> CaseContacts {get;set;}
    public DbSet<MediaAsset> MediaAssets {get;set;}
    public DbSet<PhotographyCompany> PhotographyCompanies {get;set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AgentListingCase>()
            .HasKey(alc => new { alc.AgentId, alc.ListingCaseId });

        modelBuilder.Entity<AgentPhotographyCompany>()
            .HasKey(apc => new { apc.AgentId, apc.PhotographyCompanyId });

        modelBuilder.Entity<ListingCase>()
            .Property(l => l.Latitude)
            .HasPrecision(9,6);

        modelBuilder.Entity<ListingCase>()
            .Property(l => l.Longitude)
            .HasPrecision(9,6);
    }

}
