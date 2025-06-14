using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Billing.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public OrganizationType Type { get; set; }
    public OrganizationMemberVisibilityPolicy MemberVisibilityPolicy { get; set; }

    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public ICollection<OrganizationOffering> OrganizationOfferings { get; set; } = [];
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
}
