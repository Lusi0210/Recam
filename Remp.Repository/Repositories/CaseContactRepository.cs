using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class CaseContactRepository : ICaseContactRepository
{
    private readonly RempDbContext _dbcontext;

    public CaseContactRepository(RempDbContext dbcontext)
    {
        _dbcontext=dbcontext;
    }
    public async Task<CaseContact> CreateCaseContactAsync(CaseContact caseContact)
    {
        _dbcontext.CaseContacts.Add(caseContact);
        await _dbcontext.SaveChangesAsync();
        return caseContact;
    }
    public async Task<List<CaseContact>> GetAllCaseContactAsync(int listingCaseId)
    {
        return await _dbcontext.CaseContacts.Where(x => x.ListingCaseId==listingCaseId).ToListAsync();
    }
}
