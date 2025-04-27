using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Pagination;
using HotChocolate.Types.Relay;
using Team.Shared.Models;

// ReSharper disable ClassNeverInstantiated.Global

namespace Team.Api.GraphQL;

[GraphQLName("AcceptInvitationToJoinTeamInput")]
public class AcceptInvitationToJoinTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}

[GraphQLName("AcceptInvitationToJoinTeamPayload")]
public class AcceptInvitationToJoinTeamPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("AddTeamInput")]
public class AddTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("primaryLocationId")] public string? PrimaryLocationId { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; } = [];
    [GraphQLName("organizationMemberIds")] public IEnumerable<string> OrganizationMemberIds { get; set; } = [];
}

[GraphQLName("CancelInvitationToJoinTeamInput")]
public class CancelInvitationToJoinTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}

[GraphQLName("CancelInvitationToJoinTeamPayload")]
public class CancelInvitationToJoinTeamPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("ChangeTeamMemberRoleInput")]
public class ChangeTeamMemberRoleInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("role")] public TeamMemberRole Role { get; set; }
}

[GraphQLName("DeleteTeamInput")]
public class DeleteTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}

[GraphQLName("RemoveTeamMemberInput")]
public class RemoveTeamMemberInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}

[GraphQLName("InviteCustomersToJoinTeamInput")]
public class InviteCustomersToJoinTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("teamId")] public string TeamId { get; set; } = string.Empty;
    [GraphQLName("emails")] public IEnumerable<string> Emails { get; set; } = [];
}

[GraphQLName("InviteCustomersToJoinTeamPayload")]
public class InviteCustomersToJoinTeamPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("RejectInvitationToJoinTeamInput")]
public class RejectInvitationToJoinTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}

[GraphQLName("RejectInvitationToJoinTeamPayload")]
public class RejectInvitationToJoinTeamPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("TeamConnection")]
public class TeamConnection : Enterprise.Shared.GraphQL.Types.Connection<TeamEdge>;

[GraphQLName("Team_CustomerDetails")]
public class CustomerDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("email")] public string? Email { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("givenName")] public string? GivenName { get; set; }
    [GraphQLName("middleName")] public string? MiddleName { get; set; }
    [GraphQLName("familyName")] public string? FamilyName { get; set; }
    [GraphQLName("photoUrl")] public string? PhotoUrl { get; set; }
    [GraphQLName("photoUrl24")] public string? PhotoUrl24 { get; set; }
    [GraphQLName("photoUrl32")] public string? PhotoUrl32 { get; set; }
    [GraphQLName("photoUrl48")] public string? PhotoUrl48 { get; set; }
    [GraphQLName("photoUrl72")] public string? PhotoUrl72 { get; set; }
    [GraphQLName("photoUrl192")] public string? PhotoUrl192 { get; set; }
    [GraphQLName("photoUrl512")] public string? PhotoUrl512 { get; set; }
    [GraphQLName("phoneNumber")] public string? PhoneNumber { get; set; }
}

[GraphQLName("TeamDetails")]
public class TeamDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("members")] public IEnumerable<TeamMemberDetails> Members { get; set; } = [];
    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; } = new();
    [GraphQLName("primaryLocation")] public LocationDetails? PrimaryLocation { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("hasFutureBooking")] public bool HasFutureBooking { get; set; }
    [GraphQLName("canModify")] public bool CanModify { get; set; }
    [GraphQLName("canDelete")] public bool CanDelete { get; set; }
    [GraphQLName("canInvitePeople")] public bool CanInvitePeople { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}

[GraphQLName("TeamEdge")]
public class TeamEdge(TeamDetails node, string cursor) : Edge<TeamDetails>(node, cursor);

[GraphQLName("TeamMemberConnection")]
public class TeamMemberConnection : Enterprise.Shared.GraphQL.Types.Connection<TeamMemberEdge>;

[GraphQLName("TeamMemberDetails")]
public class TeamMemberDetails : Node
{
    [GraphQLName("role")] public TeamMemberRole? Role { get; set; }
    [GraphQLName("status")] public TeamMemberStatus Status { get; set; }
    [GraphQLName("customer")] public CustomerDetails Customer { get; set; } = new();
    [GraphQLName("organizationMember")] public TeamOrganizationMemberDetails? OrganizationMember { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}

[GraphQLName("TeamMemberDetailsPayload")]
public class TeamMemberDetailsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("member")] public TeamMemberDetails? Member { get; set; }
}

[GraphQLName("TeamMemberEdge")]
public class TeamMemberEdge(TeamMemberDetails node, string cursor) : Edge<TeamMemberDetails>(node, cursor);

[GraphQLName("TeamMemberOrderInput")]
public class TeamMemberOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public TeamMemberOrderField Field { get; set; }
}

[GraphQLName("TeamMemberWhereInput")]
public class TeamMemberWhereInput
{
    [GraphQLName("teamId")] public string TeamId { get; set; } = string.Empty;
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

[GraphQLName("TeamOrderInput")]
public class TeamOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public TeamOrderField Field { get; set; }
}

[GraphQLName("Team_OrganizationDetails")]
public class OrganizationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }
}

[GraphQLName("TeamOrganizationMemberDetails")]
public class TeamOrganizationMemberDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("customer")] public CustomerDetails Customer { get; set; } = new();
}

[GraphQLName("TeamPayload")]
public class TeamPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("team")] public TeamDetails Team { get; set; } = new();
}

[GraphQLName("TeamMemberPayload")]
public class TeamMemberPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("teamMember")] public TeamMemberDetails TeamMember { get; set; } = new();
}

[GraphQLName("TeamWhereInput")]
public class TeamWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("primaryLocationIds")] public IEnumerable<string>? PrimaryLocationIds { get; set; }
}

[GraphQLName("UpdateTeamInput")]
public class UpdateTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("primaryLocationId")] public string? PrimaryLocationId { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
}

[GraphQLName("UpdateTeamAndTeamMembersInput")]
public class UpdateTeamAndTeamMembersInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("primaryLocationId")] public string? PrimaryLocationId { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; } = [];
    [GraphQLName("organizationMemberIds")] public IEnumerable<string> OrganizationMemberIds { get; set; } = [];
}

[GraphQLName("UpdateTeamMembersInput")]
public class UpdateTeamMembersInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("customerIds")] public IEnumerable<string> CustomerIds { get; set; } = [];
    [GraphQLName("organizationMemberIds")] public IEnumerable<string> OrganizationMemberIds { get; set; } = [];
}

[GraphQLName("Team_LocationDetails")]
public class LocationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}

[GraphQLName("ChangeTeamMembersStatusInput")]
public class ChangeTeamMembersStatusInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
    [GraphQLName("status")] public TeamMemberStatus Status { get; set; }
}

[GraphQLName("TeamMembersDetailsPayload")]
public class TeamMembersDetailsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("members")] public IEnumerable<TeamMemberDetails> Members { get; set; } = [];
}

[GraphQLName("RemoveTeamMembersInput")]
public class RemoveTeamMembersInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
}

[GraphQLName("AddTeamMemberInput")]
public class AddTeamMemberInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("customerId")] public string? CustomerId { get; set; }
    [GraphQLName("organizationMemberId")] public string? OrganizationMemberId { get; set; }
}

[GraphQLName("CustomerTeamWhereInput")]
public class CustomerTeamWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
    [GraphQLName("customerId")] public string CustomerId { get; set; } = string.Empty;
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("primaryLocationIds")] public IEnumerable<string>? PrimaryLocationIds { get; set; }
}
