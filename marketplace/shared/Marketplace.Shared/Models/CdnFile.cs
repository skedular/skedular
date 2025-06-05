using Enterprise.Shared.Models;

namespace Marketplace.Shared.Models;

public class CdnFile : ReplicatedModelBase
{
    public string Url { get; set; } = string.Empty;
}
