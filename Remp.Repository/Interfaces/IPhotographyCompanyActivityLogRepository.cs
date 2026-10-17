using System;
using Remp.Models.Documents;
using Remp.Models.Enums;

namespace Remp.Repository.Interfaces;

public interface IPhotographyCompanyActivityLogRepository
{
    Task AddAsync(PhotographyCompanyActivityLog log);
}
