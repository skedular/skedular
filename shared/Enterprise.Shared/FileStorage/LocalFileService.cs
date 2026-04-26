using Enterprise.Shared.Configurations;
using Flurl;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Path = System.IO.Path;

namespace Enterprise.Shared.FileStorage;

public class LocalFileService(
    ApplicationConfiguration applicationConfiguration,
    FileStorageConfiguration fileStorageConfiguration,
    ILogger<LocalFileService> logger)
    : IFileService
{
    public async Task<Uri> UploadAsync(Stream stream, string contentType, string fileName, string? extension, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Uploading file to local storage. FileName={FileName}, ContentType={ContentType}, Extension={Extension}",
            fileName,
            contentType,
            extension);

        stream.Position = 0;
        fileName = string.IsNullOrWhiteSpace(extension) ? fileName : $"{fileName}{extension}";

        var fullPath = Path.Combine(fileStorageConfiguration.FileServerFilePath, fileName);

        await using var fileStream = File.Create(fullPath);
        await stream.CopyToAsync(fileStream, cancellationToken);

        logger.LogInformation("File uploaded to local storage. FileName={FileName}", fileName);

        return new Uri(Url.Combine(applicationConfiguration.ApiBaseDomain.ToString(), fileStorageConfiguration.FileEndpoint, fileName));
    }

    public async Task<(bool, string, byte[])> GetAsync(string fileName, CancellationToken cancellationToken)
    {
        logger.LogDebug("Reading file from local storage. FileName={FileName}", fileName);

        var fullPath = Path.Combine(fileStorageConfiguration.FileServerFilePath, fileName);
        if (!Path.Exists(fullPath))
        {
            logger.LogWarning("File not found in local storage. FileName={FileName}", fileName);
            return (false, string.Empty, []);
        }

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fullPath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        logger.LogDebug("File read succeeded from local storage. FileName={FileName}, ContentType={ContentType}", fileName, contentType);
        return (true, contentType, await File.ReadAllBytesAsync(fullPath, cancellationToken));
    }
}
