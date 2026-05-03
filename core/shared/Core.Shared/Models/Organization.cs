using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Core.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? CustomDomain { get; set; }
    public OrganizationType Type { get; set; }
    public bool? IsOwnershipVerified { get; set; }
    public IReadOnlyList<OrganizationMember> OrganizationMembers { get; set; } = [];
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
}
