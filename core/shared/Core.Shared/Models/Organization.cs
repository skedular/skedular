using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Core.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? UniqueAlphanumericName { get; set; }
    public OrganizationType Type { get; set; }
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
}
