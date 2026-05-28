using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class AzureInstallStateUserIdLookup : ModelBase
{
    public string InstalledByUserId { get; set; } = string.Empty;
}
