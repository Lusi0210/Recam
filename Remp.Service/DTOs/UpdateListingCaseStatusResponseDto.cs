using System;
using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class UpdateListingCaseStatusResponseDto
{
    public int Id { get; set; }
    public ListcaseStatus PreviousStatus { get; set; }
    public ListcaseStatus CurrentStatus { get; set; }
}
