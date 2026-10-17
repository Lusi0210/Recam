using System;
using Microsoft.EntityFrameworkCore;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class PhotographyCompanyRepository : IPhotographyCompanyRepository
{
    private readonly RempDbContext _dbContext;
    public PhotographyCompanyRepository(RempDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PhotographyCompany?> GetPhotographyCompanyByIdAsync(string id)
    {
        return await _dbContext.PhotographyCompanies.FirstOrDefaultAsync(p => p.Id == id);
    }
}
