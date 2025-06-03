using Enterprise.Shared.Models;

namespace Core.Shared.Models;

public class CdnFile : ReplicatedModelBase
{
    public required Uri StorageUrl { get; set; }
    public required Uri CdnUrl { get; set; }
}
