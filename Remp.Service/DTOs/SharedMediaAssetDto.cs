using System;
using Remp.Models.Enums;

namespace Remp.Service.DTOs;

public class SharedMediaAssetDto
{
    public int Id { get; set; }
    public MediaType MediaType { get; set; }
    public string MediaUrl { get; set; } = null!;
    public bool IsHero { get; set; }
}
