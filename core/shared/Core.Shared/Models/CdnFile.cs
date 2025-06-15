using Enterprise.Shared.Models;

namespace Core.Shared.Models;

public class CdnFile : ModelBase
{
    public required Uri StorageUrl { get; set; }
    public required Uri CdnUrl { get; set; }
    public string? ContentType { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public Customer UploadedBy { get; set; } = new();
}
