using System;
using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IPhotographyCompanyRepository
{
    Task<PhotographyCompany?> GetPhotographyCompanyByIdAsync(string id);
}
