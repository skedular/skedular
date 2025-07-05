namespace Enterprise.Shared.FileStorage;

public interface ICdnService
{
    Task<(Uri, Uri)> UploadAsync(Stream stream, string contentType, string fileName, string? extension, CancellationToken cancellationToken);
    Task<(bool, string, byte[])> GetAsync(string fileName, CancellationToken cancellationToken);
}
