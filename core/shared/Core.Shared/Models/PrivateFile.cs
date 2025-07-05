using Enterprise.Shared.Models;

namespace Core.Shared.Models;

public class PrivateFile : ModelBase
{
    public required Uri StorageUrl { get; set; }
    public string? ContentType { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public Uri? ThumbnailStorageUrl { get; set; }
    public string? ThumbnailContentType { get; set; }
    public int? ThumbnailWidth { get; set; }
    public int? ThumbnailHeight { get; set; }
    public Customer UploadedBy { get; set; } = new();
}
