using System;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface IListingCaseService
{
    Task<int> CreateListingCaseAsync(CreateListingCaseDto dto,string userId,string? ipAddress);
}
