using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace Remp.Service.Interfaces;

public interface IBlobStorageService
{
    // Upload one file and return its blob URL (without SAS).
    Task<string> UploadFileAsync(IFormFile file);
 
    // Download a file by its blob URL.
    Task<(Stream Content, string ContentType, string FileName)> DownloadFileAsync(string blobUrl);
 
    // Delete a file by its blob URL.
    Task<bool> DeleteFileAsync(string blobUrl);
 
    // Generate a short-lived read-only SAS URL for secure client access.
    string GenerateSasUrl(string blobUrl, int expiryMinutes = 60);
}
