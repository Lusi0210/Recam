using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Remp.Common;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;
    private readonly BlobSettings _settings;
 
    private static readonly string[] AllowedExtensions = new[]
    {
        // Images
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp",
        // Videos
        ".mp4", ".mov", ".avi", ".mkv", ".webm",
        // Floor plan / documents
        ".pdf",
        // VR files
        ".gltf", ".glb", ".vr"
    };
 
    public BlobStorageService(IOptions<BlobSettings> options)
    {
        _settings = options.Value;
        var serviceClient = new BlobServiceClient(_settings.ConnectionString);
        _containerClient = serviceClient.GetBlobContainerClient(_settings.ContainerName);
        _containerClient.CreateIfNotExists();
    }
 
    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is empty.");
        }
 
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException($"File type '{extension}' is not allowed.");
        }
 
        // Generate a unique blob name to avoid collisions.
        var blobName = $"{Guid.NewGuid()}{extension}";
        var blobClient = _containerClient.GetBlobClient(blobName);
 
        var headers = new BlobHttpHeaders
        {
            ContentType = file.ContentType
        };
 
        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = headers
        });
 
        return blobClient.Uri.ToString();
    }
 
    public async Task<(Stream Content, string ContentType, string FileName)> DownloadFileAsync(string blobUrl)
    {
        var blobClient = GetBlobClientFromUrl(blobUrl);
 
        if (!await blobClient.ExistsAsync())
        {
            throw new FileNotFoundException("The requested blob does not exist.");
        }
 
        var download = await blobClient.DownloadContentAsync();
        var contentType = download.Value.Details.ContentType ?? "application/octet-stream";
        var fileName = blobClient.Name;
 
        var stream = new MemoryStream(download.Value.Content.ToArray());
        return (stream, contentType, fileName);
    }
 
    public async Task<bool> DeleteFileAsync(string blobUrl)
    {
        var blobClient = GetBlobClientFromUrl(blobUrl);
        var result = await blobClient.DeleteIfExistsAsync();
        return result.Value;
    }
 
    public string GenerateSasUrl(string blobUrl, int expiryMinutes = 60)
    {
        var blobClient = GetBlobClientFromUrl(blobUrl);
 
        if (!blobClient.CanGenerateSasUri)
        {
            // CanGenerateSasUri is true only when the client is built from
            // account-key credentials (our connection string), so this path
            // should not happen in normal use.
            throw new InvalidOperationException("Cannot generate SAS URL for this blob client.");
        }
 
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerClient.Name,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);
 
        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }
 
    private BlobClient GetBlobClientFromUrl(string blobUrl)
    {
        // The blob name is the last segment of the URL.
        var uri = new Uri(blobUrl);
        var blobName = Path.GetFileName(uri.LocalPath);
        return _containerClient.GetBlobClient(blobName);
    }
}
