using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace MsTeams.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? CustomDomain { get; set; }
    public bool? IsOwnershipVerified { get; set; }
    public OrganizationType Type { get; set; }
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public ICollection<AzureTenant> AzureTenants { get; set; } = [];
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
}
