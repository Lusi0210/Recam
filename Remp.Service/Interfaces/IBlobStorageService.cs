using System;

namespace Remp.Service.Interfaces;

public interface IBlobStorageService
{
    Task UploadAsync(Stream content, string blobName, string contentType);
    Task DeleteAsync(string blobName);
    string GetReadUrl(string blobName);
    Task<(Stream Content, string ContentType, string FileName)> DownloadFileAsync(string blobName);
}
