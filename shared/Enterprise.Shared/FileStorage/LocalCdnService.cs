using Enterprise.Shared.Configurations;
using Flurl;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Path = System.IO.Path;

namespace Enterprise.Shared.FileStorage;

public class LocalCdnService(
    ApplicationConfiguration applicationConfiguration,
    FileStorageConfiguration fileStorageConfiguration,
    ILogger<LocalCdnService> logger)
    : ICdnService
{
    public async Task<(Uri, Uri)> UploadAsync(
        Stream stream,
        string contentType,
        string fileName,
        string? extension,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Uploading CDN file to local storage. FileName={FileName}, ContentType={ContentType}, Extension={Extension}",
            fileName,
            contentType,
            extension);

        stream.Position = 0;
        fileName = string.IsNullOrWhiteSpace(extension) ? fileName : $"{fileName}{extension}";

        var fullPath = Path.Combine(fileStorageConfiguration.FileServerPublicFilePath, fileName);

        await using var fileStream = File.Create(fullPath);
        await stream.CopyToAsync(fileStream, cancellationToken);

        var uri = new Uri(Url.Combine(applicationConfiguration.ApiBaseDomain.ToString(), fileStorageConfiguration.PublicCdnFileEndpoint, fileName));
        logger.LogInformation("CDN file uploaded to local storage. FileName={FileName}", fileName);
        return new ValueTuple<Uri, Uri>(uri, uri);
    }

    public async Task<(bool, string, byte[])> GetAsync(string fileName, CancellationToken cancellationToken)
    {
        logger.LogDebug("Reading CDN file from local storage. FileName={FileName}", fileName);

        var fullPath = Path.Combine(fileStorageConfiguration.FileServerPublicFilePath, fileName);
        if (!Path.Exists(fullPath))
        {
            logger.LogWarning("CDN file not found in local storage. FileName={FileName}", fileName);
            return (false, string.Empty, []);
        }

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fullPath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        logger.LogDebug("CDN file read succeeded from local storage. FileName={FileName}, ContentType={ContentType}", fileName, contentType);
        return (true, contentType, await File.ReadAllBytesAsync(fullPath, cancellationToken));
    }
}
