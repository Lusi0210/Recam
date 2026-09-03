using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface ICaseContactRepository
{
    Task<CaseContact> CreateCaseContactAsync(CaseContact caseContact);
    Task<List<CaseContact>> GetAllCaseContactAsync(int listingCaseId);
}
