using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.Api.Services;

public sealed class BlobStorageService
{
    private readonly BlobContainerClient _container;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"];
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("AzureBlobStorage:ConnectionString is missing.");
        _container = new BlobContainerClient(connectionString, configuration["AzureBlobStorage:ContainerName"] ?? "raceday-images");
    }

    public async Task<string> UploadAsync(IFormFile file, string folder)
    {
        if (file.Length == 0) throw new ArgumentException("The uploaded file is empty.");
        await _container.CreateIfNotExistsAsync();
        var blob = _container.GetBlobClient($"{folder}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
        await using var stream = file.OpenReadStream();
        await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
        return blob.Uri.ToString();
    }
}
