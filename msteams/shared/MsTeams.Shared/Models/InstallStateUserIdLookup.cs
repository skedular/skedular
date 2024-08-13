using Enterprise.Shared.Models;

namespace MsTeams.Shared.Models;

public class InstallStateUserIdLookup : ModelBase
{
    public string InstalledByUserId { get; set; } = string.Empty;
}
