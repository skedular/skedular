using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Marketplace.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public Offering? Offering { get; set; }
    public OrganizationType Type { get; set; }
    public OrganizationMemberVisibilityPolicy MemberVisibilityPolicy { get; set; }
    public ICollection<OrganizationTag> Tags { get; set; } = [];
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public ICollection<Product> Products { get; set; } = [];
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
}
