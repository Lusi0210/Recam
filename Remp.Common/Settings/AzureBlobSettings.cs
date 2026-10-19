using System;

namespace Remp.Common.Settings;

public class AzureBlobSettings
{
    public string ConnectionString { get; set; } = null!;
    public string ContainerName { get; set; } = null!;
    public int SasExpiryMinutes { get; set; } = 60;
}
