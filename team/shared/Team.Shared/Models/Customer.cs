using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Team.Shared.Models;

public class Customer : ReplicatedModelBaseWithDeleted
{
    public ICollection<Identity> Identities { get; set; } = [];
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public ICollection<TeamMember> TeamMembers { get; set; } = [];
    public ICollection<JoinInvitation> JoinInvitationsCreatedBy { get; set; } = [];
    public ICollection<JoinInvitation> JoinInvitationsInvitee { get; set; } = [];
    public string? Name { get; set; }
    public string? GivenName { get; set; }
    public string? MiddleName { get; set; }
    public string? FamilyName { get; set; }
    public CustomerType? Type { get; set; }
}
