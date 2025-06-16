using Microsoft.AspNetCore.StaticFiles;
using Path = System.IO.Path;

namespace Enterprise.Shared.Cdn;

public class LocalCdnService(CdnConfiguration cdnConfiguration) : ICdnService
{
    public async Task<(Uri, Uri)> UploadAsync(
        Stream stream,
        string contentType,
        string fileName,
        string? extension,
        CancellationToken cancellationToken)
    {
        stream.Position = 0;
        fileName = string.IsNullOrWhiteSpace(extension) ? fileName : $"{fileName}{extension}";

        var fullPath = Path.Combine(cdnConfiguration.LocalCdnPath, fileName);

        await using var fileStream = File.Create(fullPath);
        await stream.CopyToAsync(fileStream, cancellationToken);

        var uri = new Uri(cdnConfiguration.LocalCdnBaseUri, fileName);
        return new ValueTuple<Uri, Uri>(uri, uri);
    }

    public async Task<(bool, string, byte[])> GetAsync(string fileName, CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(cdnConfiguration.LocalCdnPath, fileName);
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
