using HotChocolate;
using HotChocolate.Types.Relay;

namespace Team.Api.GraphQL;

[GraphQLName("AcceptInvitationToJoinTeamInput")]
public class AcceptInvitationToJoinTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }
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

    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("about")] public string? About { get; set; }

    [GraphQLName("customerIds")] public string[] CustomerIds { get; set; }

    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("timezone")] public string? Timezone { get; set; }

    [GraphQLName("organizationMemberIds")] public string[] OrganizationMemberIds { get; set; }
}

[GraphQLName("CancelInvitationToJoinTeamInput")]
public class CancelInvitationToJoinTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }
}

[GraphQLName("CancelInvitationToJoinTeamPayload")]
public class CancelInvitationToJoinTeamPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("ChangeTeamMemberOwnershipTypeInput")]
public class ChangeTeamMemberOwnershipTypeInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }

    [GraphQLName("membershipType")] public TeamMemberMembershipType MembershipType { get; set; }
}

[GraphQLName("DeleteTeamInput")]
public class DeleteTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }
}

[GraphQLName("InviteCustomersToJoinTeamInput")]
public class InviteCustomersToJoinTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("teamId")] public string TeamId { get; set; }

    [GraphQLName("emails")] public string[] Emails { get; set; }
}

[GraphQLName("InviteCustomersToJoinTeamPayload")]
public class InviteCustomersToJoinTeamPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("Mutation_AcceptInvitationToJoinTeam_Arguments")]
public class Mutation_AcceptInvitationToJoinTeam_Arguments
{
    [GraphQLName("input")] public AcceptInvitationToJoinTeamInput Input { get; set; }
}

[GraphQLName("Mutation_AddTeam_Arguments")]
public class Mutation_AddTeam_Arguments
{
    [GraphQLName("input")] public AddTeamInput Input { get; set; }
}

[GraphQLName("Mutation_CancelInvitationToJoinTeam_Arguments")]
public class Mutation_CancelInvitationToJoinTeam_Arguments
{
    [GraphQLName("input")] public CancelInvitationToJoinTeamInput Input { get; set; }
}

[GraphQLName("Mutation_ChangeTeamMemberOwnershipType_Arguments")]
public class Mutation_ChangeTeamMemberOwnershipType_Arguments
{
    [GraphQLName("input")] public ChangeTeamMemberOwnershipTypeInput Input { get; set; }
}

[GraphQLName("Mutation_DeleteTeam_Arguments")]
public class Mutation_DeleteTeam_Arguments
{
    [GraphQLName("input")] public DeleteTeamInput Input { get; set; }
}

[GraphQLName("Mutation_InviteCustomersToJoinTeam_Arguments")]
public class Mutation_InviteCustomersToJoinTeam_Arguments
{
    [GraphQLName("input")] public InviteCustomersToJoinTeamInput Input { get; set; }
}

[GraphQLName("Mutation_RejectInvitationToJoinTeam_Arguments")]
public class Mutation_RejectInvitationToJoinTeam_Arguments
{
    [GraphQLName("input")] public RejectInvitationToJoinTeamInput Input { get; set; }
}

[GraphQLName("Mutation_UpdateTeam_Arguments")]
public class Mutation_UpdateTeam_Arguments
{
    [GraphQLName("input")] public UpdateTeamInput Input { get; set; }
}

[GraphQLName("Node")]
public interface Node
{
    [GraphQLName("id")] [ID] public string Id { get; set; }
}

public enum OrderDirection
{
    Ascending,
    Descending
}

[GraphQLName("PageInfo")]
public class PageInfo
{
    [GraphQLName("hasNextPage")] public bool HasNextPage { get; set; }

    [GraphQLName("hasPreviousPage")] public bool HasPreviousPage { get; set; }

    [GraphQLName("startCursor")] public string? StartCursor { get; set; }

    [GraphQLName("endCursor")] public string? EndCursor { get; set; }
}

[GraphQLName("Query_MyTeams_Arguments")]
public class Query_MyTeams_Arguments
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
}

[GraphQLName("Query_PaginatedTeamMembers_Arguments")]
public class Query_PaginatedTeamMembers_Arguments
{
    [GraphQLName("after")] public string? After { get; set; }

    [GraphQLName("first")] public int? First { get; set; }

    [GraphQLName("before")] public string? Before { get; set; }

    [GraphQLName("last")] public int? Last { get; set; }

    [GraphQLName("where")] public TeamMemberWhereInput Where { get; set; }

    [GraphQLName("orderBy")] public TeamMemberOrderInput[]? OrderBy { get; set; }
}

[GraphQLName("Query_Team_Arguments")]
public class Query_Team_Arguments
{
    [GraphQLName("id")] public string Id { get; set; }
}

[GraphQLName("Query_TeamMembers_Arguments")]
public class Query_TeamMembers_Arguments
{
    [GraphQLName("where")] public TeamMemberWhereInput Where { get; set; }

    [GraphQLName("orderBy")] public TeamMemberOrderInput[]? OrderBy { get; set; }
}

[GraphQLName("Query_Teams_Arguments")]
public class Query_Teams_Arguments
{
    [GraphQLName("after")] public string? After { get; set; }

    [GraphQLName("first")] public int? First { get; set; }

    [GraphQLName("before")] public string? Before { get; set; }

    [GraphQLName("last")] public int? Last { get; set; }

    [GraphQLName("where")] public TeamWhereInput Where { get; set; }

    [GraphQLName("orderBy")] public TeamOrderInput[]? OrderBy { get; set; }
}

[GraphQLName("RejectInvitationToJoinTeamInput")]
public class RejectInvitationToJoinTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }
}

[GraphQLName("RejectInvitationToJoinTeamPayload")]
public class RejectInvitationToJoinTeamPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("TeamConnection")]
public class TeamConnection
{
    [GraphQLName("pageInfo")] public PageInfo PageInfo { get; set; }

    [GraphQLName("edges")] public TeamEdge[] Edges { get; set; }

    [GraphQLName("totalCount")] public int? TotalCount { get; set; }
}

[GraphQLName("TeamCustomerDetails")]
public class TeamCustomerDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

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
}

[GraphQLName("TeamDetails")]
public class TeamDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("about")] public string? About { get; set; }

    [GraphQLName("members")] public TeamMemberDetails[] Members { get; set; }

    [GraphQLName("organization")] public TeamOrganizationDetails? Organization { get; set; }

    [GraphQLName("timezone")] public string? Timezone { get; set; }

    [GraphQLName("hasFutureBooking")] public bool HasFutureBooking { get; set; }

    [GraphQLName("canModify")] public bool CanModify { get; set; }

    [GraphQLName("canDelete")] public bool CanDelete { get; set; }

    [GraphQLName("canInvitePeople")] public bool CanInvitePeople { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; }
}

[GraphQLName("TeamEdge")]
public class TeamEdge
{
    [GraphQLName("node")] public TeamDetails Node { get; set; }

    [GraphQLName("cursor")] public string Cursor { get; set; }
}

public enum TeamJoinInvitationStatus
{
    PENDING,
    ACCEPTED,
    REJECTED
}

[GraphQLName("TeamMemberConnection")]
public class TeamMemberConnection
{
    [GraphQLName("pageInfo")] public PageInfo PageInfo { get; set; }

    [GraphQLName("edges")] public TeamMemberEdge[] Edges { get; set; }

    [GraphQLName("totalCount")] public int? TotalCount { get; set; }
}

[GraphQLName("TeamMemberDetails")]
public class TeamMemberDetails : Node
{
    [GraphQLName("membershipType")] public TeamMemberMembershipType? MembershipType { get; set; }

    [GraphQLName("customer")] public TeamCustomerDetails Customer { get; set; }

    [GraphQLName("organizationMember")] public TeamOrganizationMemberDetails? OrganizationMember { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; }
}

[GraphQLName("TeamMemberDetailsPayload")]
public class TeamMemberDetailsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("member")] public TeamMemberDetails? Member { get; set; }
}

[GraphQLName("TeamMemberEdge")]
public class TeamMemberEdge
{
    [GraphQLName("node")] public TeamMemberDetails Node { get; set; }

    [GraphQLName("cursor")] public string Cursor { get; set; }
}

public enum TeamMemberMembershipType
{
    OWNER,
    ADMINISTRATOR,
    MEMBER
}

public enum TeamMemberOrderField
{
    membershipType,
    name,
    givenName,
    middleName,
    familyName
}

[GraphQLName("TeamMemberOrderInput")]
public class TeamMemberOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }

    [GraphQLName("field")] public TeamMemberOrderField Field { get; set; }
}

[GraphQLName("TeamMemberWhereInput")]
public class TeamMemberWhereInput
{
    [GraphQLName("teamId")] public string TeamId { get; set; }

    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

public enum TeamOrderField
{
    name,
    about,
    website
}

[GraphQLName("TeamOrderInput")]
public class TeamOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }

    [GraphQLName("field")] public TeamOrderField Field { get; set; }
}

[GraphQLName("TeamOrganizationDetails")]
public class TeamOrganizationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }
}

[GraphQLName("TeamOrganizationMemberDetails")]
public class TeamOrganizationMemberDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

    [GraphQLName("customer")] public TeamCustomerDetails Customer { get; set; }
}

[GraphQLName("TeamPayload")]
public class TeamPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("team")] public TeamDetails Team { get; set; }
}

[GraphQLName("TeamWhereInput")]
public class TeamWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

[GraphQLName("UpdateTeamInput")]
public class UpdateTeamInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }

    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("about")] public string? About { get; set; }

    [GraphQLName("customerIds")] public string[] CustomerIds { get; set; }

    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("timezone")] public string? Timezone { get; set; }

    [GraphQLName("organizationMemberIds")] public string[] OrganizationMemberIds { get; set; }
}

[GraphQLName("Version")]
public class Version
{
    [GraphQLName("major")] public int Major { get; set; }

    [GraphQLName("minor")] public int Minor { get; set; }

    [GraphQLName("build")] public int Build { get; set; }

    [GraphQLName("revision")] public int Revision { get; set; }
}
