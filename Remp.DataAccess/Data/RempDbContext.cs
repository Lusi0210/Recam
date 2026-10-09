using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Remp.Models.Entities;

namespace Remp.DataAccess.Data;

public class RempDbContext : IdentityDbContext<User>
{
    public RempDbContext(DbContextOptions<RempDbContext> options) : base(options)
    {

    }
    public DbSet<ListingCase> ListingCases { get; set; }
    public DbSet<CaseContact> CaseContacts { get; set; }
    public DbSet<MediaAsset> MediaAssets { get; set; }
    public DbSet<Agent> Agents { get; set; }
    public DbSet<PhotographyCompany> PhotographyCompanies { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Agent>()
            .HasOne(a => a.User)
            .WithOne(u => u.Agent)
            .HasForeignKey<Agent>(a => a.Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PhotographyCompany>()
            .HasOne(p => p.User)
            .WithOne(u => u.PhotographyCompany)
            .HasForeignKey<PhotographyCompany>(p => p.Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ListingCase>()
            .Property(x => x.Latitude)
            .HasPrecision(9, 6);

        builder.Entity<ListingCase>()
            .Property(x => x.Longitude)
            .HasPrecision(10, 6);

        builder.Entity<ListingCase>()
            .Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.Entity<MediaAsset>()
            .HasOne(m => m.User)
            .WithMany(u => u.MediaAssets)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ListingCase>()
            .HasOne(l => l.User)
            .WithMany(u => u.ListingCases)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Agent <-> ListingCase
        builder.Entity<Agent>()
            .HasMany(a => a.ListingCases)
            .WithMany(l => l.Agents)
            .UsingEntity<Dictionary<string, object>>(
                "AgentListingCase",
                r => r.HasOne<ListingCase>().WithMany().HasForeignKey("ListingCaseId"),
                l => l.HasOne<Agent>().WithMany().HasForeignKey("AgentId"));

        // Agent <-> PhotographyCompany
        builder.Entity<Agent>()
            .HasMany(a => a.PhotographyCompanies)
            .WithMany(p => p.Agents)
            .UsingEntity<Dictionary<string, object>>(
                "AgentPhotographyCompany",
                r => r.HasOne<PhotographyCompany>().WithMany().HasForeignKey("PhotographyCompanyId"),
                l => l.HasOne<Agent>().WithMany().HasForeignKey("AgentId"));
    }

}
