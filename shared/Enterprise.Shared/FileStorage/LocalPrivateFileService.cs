using Enterprise.Shared.Configurations;
using Flurl;
using Microsoft.AspNetCore.StaticFiles;
using Path = System.IO.Path;

namespace Enterprise.Shared.FileStorage;

public class LocalPrivateFileService(ApplicationConfiguration applicationConfiguration, FileStorageConfiguration fileStorageConfiguration)
    : IPrivateFileService
{
    public async Task<Uri> UploadAsync(Stream stream, string contentType, string fileName, string? extension, CancellationToken cancellationToken)
    {
        stream.Position = 0;
        fileName = string.IsNullOrWhiteSpace(extension) ? fileName : $"{fileName}{extension}";

        var fullPath = Path.Combine(fileStorageConfiguration.LocalPrivateFilePath, fileName);

        await using var fileStream = File.Create(fullPath);
        await stream.CopyToAsync(fileStream, cancellationToken);

        return new Uri(Url.Combine(applicationConfiguration.ApiBaseDomain.ToString(), fileStorageConfiguration.PrivateFileEndpoint, fileName));
    }

    public async Task<(bool, string, byte[])> GetAsync(string fileName, CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(fileStorageConfiguration.LocalPrivateFilePath, fileName);
        if (!File.Exists(fullPath))
        {
            return (false, string.Empty, []);
        }

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fullPath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return (true, contentType, await File.ReadAllBytesAsync(fullPath, cancellationToken));
    }
}
