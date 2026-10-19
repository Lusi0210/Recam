using System;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;

namespace Remp.Repository.Repositories;

public class CaseContactRepository : ICaseContactRepository
{
    private readonly RempDbContext _dbContext;

    public CaseContactRepository(RempDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(CaseContact caseContact)
    {
        _dbContext.CaseContacts.Add(caseContact);
    }
}
