namespace Enterprise.Shared.FileStorage;

public interface IFileService
{
    Task<Uri> UploadAsync(Stream stream, string contentType, string fileName, string? extension, CancellationToken cancellationToken);
    Task<(bool, string, byte[])> GetAsync(string fileName, CancellationToken cancellationToken);
}
