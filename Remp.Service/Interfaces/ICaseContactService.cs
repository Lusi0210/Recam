using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Remp.Common;
using Remp.Service.DTOs;

namespace Remp.Service.Interfaces;

public interface ICaseContactService
{
    Task<ApiResponse<int>> CreateCaseContactAsync(CreateCaseContactDto dto);
    Task<ApiResponse<List<GetAllCaseContactResponseDto>>> GetAllCaseContact(int listingCaseId);
}
