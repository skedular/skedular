using Enterprise.Shared.Configurations;
using Flurl;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Path = System.IO.Path;

namespace Enterprise.Shared.FileStorage;

public class LocalPrivateFileService(
    ApplicationConfiguration applicationConfiguration,
    FileStorageConfiguration fileStorageConfiguration,
    ILogger<LocalPrivateFileService> logger)
    : IPrivateFileService
{
    public async Task<Uri> UploadAsync(Stream stream, string contentType, string fileName, string? extension, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Uploading private file to local storage. FileName={FileName}, ContentType={ContentType}, Extension={Extension}",
            fileName,
            contentType,
            extension);

        stream.Position = 0;
        fileName = string.IsNullOrWhiteSpace(extension) ? fileName : $"{fileName}{extension}";

        var fullPath = Path.Combine(fileStorageConfiguration.LocalPrivateFilePath, fileName);

        await using var fileStream = File.Create(fullPath);
        await stream.CopyToAsync(fileStream, cancellationToken);

        logger.LogInformation("Private file uploaded to local storage. FileName={FileName}", fileName);

        return new Uri(Url.Combine(applicationConfiguration.ApiBaseDomain.ToString(), fileStorageConfiguration.PrivateFileEndpoint, fileName));
    }

    public async Task<(bool, string, byte[])> GetAsync(string fileName, CancellationToken cancellationToken)
    {
        logger.LogDebug("Reading private file from local storage. FileName={FileName}", fileName);

        var fullPath = Path.Combine(fileStorageConfiguration.LocalPrivateFilePath, fileName);
        if (!File.Exists(fullPath))
        {
            logger.LogWarning("Private file not found in local storage. FileName={FileName}", fileName);
            return (false, string.Empty, []);
        }

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fullPath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        logger.LogDebug("Private file read succeeded from local storage. FileName={FileName}, ContentType={ContentType}", fileName, contentType);
        return (true, contentType, await File.ReadAllBytesAsync(fullPath, cancellationToken));
    }
}
