using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Team.Shared.Models;

public class Customer : ReplicatedModelBaseWithDeleted
{
    public IReadOnlyList<Identity> Identities { get; set; } = [];
    public IReadOnlyList<OrganizationMember> OrganizationMembers { get; set; } = [];
    public IReadOnlyList<TeamMember> TeamMembers { get; set; } = [];
    public IReadOnlyList<JoinInvitation> JoinInvitationsCreatedBy { get; set; } = [];
    public IReadOnlyList<JoinInvitation> JoinInvitationsInvitee { get; set; } = [];
    public string? Name { get; set; }
    public string? GivenName { get; set; }
    public string? MiddleName { get; set; }
    public string? FamilyName { get; set; }
    public CustomerType? Type { get; set; }
}
