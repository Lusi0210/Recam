using System;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface ICaseContactService
{
    Task<int> AddCaseContactToListingCaseAsync(int id,AddCaseContactDto dto,string userId);
}
