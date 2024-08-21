using Enterprise.Shared.Models;

namespace MsTeams.Shared.Models;

public class AzureInstallStateUserIdLookup : ModelBase
{
    public string InstalledByUserId { get; set; } = string.Empty;
}
