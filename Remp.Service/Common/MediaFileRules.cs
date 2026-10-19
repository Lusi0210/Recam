using System;
using Remp.Models.Enums;

namespace Remp.Service.Common;

public class MediaFileRules
{
    private const long OneMegabyte = 1024 * 1024;

    public static readonly Dictionary<MediaType, HashSet<string>> AllowedExtensions = new Dictionary<MediaType, HashSet<string>>
    {
        { MediaType.Photos,   new HashSet<string> { ".jpg", ".jpeg", ".png", ".webp" } },
        { MediaType.Videography,     new HashSet<string> { ".mp4", ".mov" } },
        { MediaType.FloorPlan, new HashSet<string> { ".jpg", ".jpeg", ".png", ".pdf" } },
        { MediaType.VRTour,    new HashSet<string> { ".jpg", ".jpeg", ".mp4" } }
    };

    public static readonly Dictionary<MediaType, long> MaxFileSizeBytes = new Dictionary<MediaType, long>
    {
        { MediaType.Photos,   20 * OneMegabyte },
        { MediaType.Videography,     500 * OneMegabyte },
        { MediaType.FloorPlan, 20 * OneMegabyte },
        { MediaType.VRTour,    500 * OneMegabyte }
    };
}
