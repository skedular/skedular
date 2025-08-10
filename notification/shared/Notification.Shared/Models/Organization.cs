using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Notification.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? UniqueAlphanumericName { get; set; }
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }
    public OrganizationType Type { get; set; }
    public OrganizationMemberVisibilityPolicy MemberVisibilityPolicy { get; set; }

    public ICollection<Location> Locations { get; set; } = [];
    public ICollection<Team> Teams { get; set; } = [];
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
}
