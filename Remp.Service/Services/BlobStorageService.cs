using System;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using Remp.Common.Exceptions;
using Remp.Common.Settings;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;
    private readonly AzureBlobSettings _settings;
    public BlobStorageService(BlobServiceClient blobServiceClient, IOptions<AzureBlobSettings> options)
    {
        _settings = options.Value;
        _containerClient = blobServiceClient.GetBlobContainerClient(_settings.ContainerName);
    }

    public async Task UploadAsync(Stream content, string blobName, string contentType)
    {
        BlobClient blobClient = _containerClient.GetBlobClient(blobName);

        BlobUploadOptions uploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        await blobClient.UploadAsync(content, uploadOptions);
    }

    public async Task DeleteAsync(string blobName)
    {
        BlobClient blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }

    public string GetReadUrl(string blobName)
    {
        BlobClient blobClient = _containerClient.GetBlobClient(blobName);

        BlobSasBuilder sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerClient.Name,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(_settings.SasExpiryMinutes)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
        return sasUri.ToString();
    }

    public async Task<(Stream Content, string ContentType, string FileName)> DownloadFileAsync(string blobName)
    {
        BlobClient blobClient = _containerClient.GetBlobClient(blobName);

        try
        {
            Response<BlobDownloadStreamingResult> response = await blobClient.DownloadStreamingAsync();
            Stream content = response.Value.Content;
            string contentType = response.Value.Details.ContentType ?? "application/octet-stream";
            string fileName = Path.GetFileName(blobName);

            return (content, contentType, fileName);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            throw new NotFoundException("The file does not exist in storage.");
        }
    }

}
