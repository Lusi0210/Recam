using System;

namespace Remp.Service.DTOs;

public class PaginationQueryDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
