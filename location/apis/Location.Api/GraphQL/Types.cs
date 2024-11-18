using HotChocolate;
using HotChocolate.Types.Relay;

namespace Location.Api.GraphQL;

[GraphQLName("AcceptInvitationToJoinLocationInput")]
public class AcceptInvitationToJoinLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }
}

[GraphQLName("AcceptInvitationToJoinLocationPayload")]
public class AcceptInvitationToJoinLocationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("AddDeskInput")]
public class AddDeskInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string? Id { get; set; }

    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("locationId")] public string LocationId { get; set; }

    [GraphQLName("locationTagIds")] public string[] LocationTagIds { get; set; }
}

[GraphQLName("AddLocationInput")]
public class AddLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string? Id { get; set; }

    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("about")] public string? About { get; set; }

    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("timezone")] public string? Timezone { get; set; }
}

[GraphQLName("AddLocationTagInput")]
public class AddLocationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string? Id { get; set; }

    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("description")] public string? Description { get; set; }

    [GraphQLName("tagType")] public string TagType { get; set; }

    [GraphQLName("locationId")] public string LocationId { get; set; }
}

[GraphQLName("BulkAddDeskInput")]
public class BulkAddDeskInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string? Id { get; set; }

    [GraphQLName("namePrefix")] public string? NamePrefix { get; set; }

    [GraphQLName("locationId")] public string LocationId { get; set; }

    [GraphQLName("count")] public int Count { get; set; }

    [GraphQLName("locationTagIds")] public string[] LocationTagIds { get; set; }

    [GraphQLName("deactivated")] public bool Deactivated { get; set; }

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }
}

[GraphQLName("BulkDeskPayload")]
public class BulkDeskPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("desks")] public DeskDetails[] Desks { get; set; }
}

[GraphQLName("CancelInvitationToJoinLocationInput")]
public class CancelInvitationToJoinLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }
}

[GraphQLName("CancelInvitationToJoinLocationPayload")]
public class CancelInvitationToJoinLocationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("ChangeLocationMemberOwnershipTypeInput")]
public class ChangeLocationMemberOwnershipTypeInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }

    [GraphQLName("membershipType")] public LocationMemberMembershipType MembershipType { get; set; }
}

[GraphQLName("DeleteDeskInput")]
public class DeleteDeskInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }
}

[GraphQLName("DeleteLocationInput")]
public class DeleteLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }
}

[GraphQLName("DeleteLocationTagInput")]
public class DeleteLocationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }
}

[GraphQLName("DeskConnection")]
public class DeskConnection
{
    [GraphQLName("pageInfo")] public PageInfo PageInfo { get; set; }

    [GraphQLName("edges")] public DeskEdge[] Edges { get; set; }

    [GraphQLName("totalCount")] public int? TotalCount { get; set; }
}

[GraphQLName("DeskDetails")]
public class DeskDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("deactivated")] public bool Deactivated { get; set; }

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("locationTags")] public LocationTagDetails[] LocationTags { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; }
}

[GraphQLName("DeskEdge")]
public class DeskEdge
{
    [GraphQLName("node")] public DeskDetails Node { get; set; }

    [GraphQLName("cursor")] public string Cursor { get; set; }
}

public enum DeskOrderField
{
    Name
}

[GraphQLName("DeskOrderInput")]
public class DeskOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }

    [GraphQLName("field")] public DeskOrderField Field { get; set; }
}

[GraphQLName("DeskPayload")]
public class DeskPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("desk")] public DeskDetails Desk { get; set; }
}

[GraphQLName("DeskWhereInput")]
public class DeskWhereInput
{
    [GraphQLName("locationId")] public string LocationId { get; set; }

    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

[GraphQLName("InviteCustomersToJoinLocationInput")]
public class InviteCustomersToJoinLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("locationId")] public string LocationId { get; set; }

    [GraphQLName("emails")] public string[] Emails { get; set; }
}

[GraphQLName("InviteCustomersToJoinLocationPayload")]
public class InviteCustomersToJoinLocationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("LocationAnalytics")]
public class LocationAnalytics
{
    [GraphQLName("desksOccupancyPercentage")]
    public LocationDesksOccupancyPercentage[] DesksOccupancyPercentage { get; set; }

    [GraphQLName("dailyBookingsTotals")] public LocationDailyBookingsTotal[] DailyBookingsTotals { get; set; }
}

[GraphQLName("LocationConnection")]
public class LocationConnection
{
    [GraphQLName("pageInfo")] public PageInfo PageInfo { get; set; }

    [GraphQLName("edges")] public LocationEdge[] Edges { get; set; }

    [GraphQLName("totalCount")] public int? TotalCount { get; set; }
}

[GraphQLName("LocationCustomerDetails")]
public class LocationCustomerDetails
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

[GraphQLName("LocationDailyBookingsTotal")]
public class LocationDailyBookingsTotal
{
    [GraphQLName("date")] public DateTimeOffset Date { get; set; }

    [GraphQLName("total")] public int Total { get; set; }
}

[GraphQLName("LocationDesksOccupancyPercentage")]
public class LocationDesksOccupancyPercentage
{
    [GraphQLName("date")] public DateTimeOffset Date { get; set; }

    [GraphQLName("percentage")] public float Percentage { get; set; }
}

[GraphQLName("LocationDetails")]
public class LocationDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("about")] public string? About { get; set; }

    [GraphQLName("organization")] public LocationOrganizationDetails? Organization { get; set; }

    [GraphQLName("timezone")] public string? Timezone { get; set; }

    [GraphQLName("deskCapacity")] public int DeskCapacity { get; set; }

    [GraphQLName("hasFutureBooking")] public bool HasFutureBooking { get; set; }

    [GraphQLName("canModify")] public bool CanModify { get; set; }

    [GraphQLName("canDelete")] public bool CanDelete { get; set; }

    [GraphQLName("canInvitePeople")] public bool CanInvitePeople { get; set; }

    [GraphQLName("canViewAnalytics")] public bool CanViewAnalytics { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; }
}

[GraphQLName("LocationEdge")]
public class LocationEdge
{
    [GraphQLName("node")] public LocationDetails Node { get; set; }

    [GraphQLName("cursor")] public string Cursor { get; set; }
}

[GraphQLName("LocationMemberConnection")]
public class LocationMemberConnection
{
    [GraphQLName("pageInfo")] public PageInfo PageInfo { get; set; }

    [GraphQLName("edges")] public LocationMemberEdge[] Edges { get; set; }

    [GraphQLName("totalCount")] public int? TotalCount { get; set; }
}

[GraphQLName("LocationMemberDetails")]
public class LocationMemberDetails : Node
{
    [GraphQLName("membershipType")] public LocationMemberMembershipType? MembershipType { get; set; }

    [GraphQLName("customer")] public LocationCustomerDetails Customer { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; }
}

[GraphQLName("LocationMemberDetailsPayload")]
public class LocationMemberDetailsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("member")] public LocationMemberDetails? Member { get; set; }
}

[GraphQLName("LocationMemberEdge")]
public class LocationMemberEdge
{
    [GraphQLName("node")] public LocationMemberDetails Node { get; set; }

    [GraphQLName("cursor")] public string Cursor { get; set; }
}

public enum LocationMemberMembershipType
{
    Owner,
    Administrator,
    Member
}

public enum LocationMemberOrderField
{
    MembershipType,
    Name,
    GivenName,
    MiddleName,
    FamilyName
}

[GraphQLName("LocationMemberOrderInput")]
public class LocationMemberOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }

    [GraphQLName("field")] public LocationMemberOrderField Field { get; set; }
}

[GraphQLName("LocationMemberWhereInput")]
public class LocationMemberWhereInput
{
    [GraphQLName("locationId")] public string LocationId { get; set; }

    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

public enum LocationOrderField
{
    Name
}

[GraphQLName("LocationOrderInput")]
public class LocationOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }

    [GraphQLName("field")] public LocationOrderField Field { get; set; }
}

[GraphQLName("LocationOrganizationDetails")]
public class LocationOrganizationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; }

    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }
}

[GraphQLName("LocationPayload")]
public class LocationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("location")] public LocationDetails Location { get; set; }
}

[GraphQLName("LocationTagConnection")]
public class LocationTagConnection
{
    [GraphQLName("pageInfo")] public PageInfo PageInfo { get; set; }

    [GraphQLName("edges")] public LocationTagEdge[] Edges { get; set; }

    [GraphQLName("totalCount")] public int? TotalCount { get; set; }
}

[GraphQLName("LocationTagDetails")]
public class LocationTagDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("description")] public string? Description { get; set; }

    [GraphQLName("tagType")] public string TagType { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; }
}

[GraphQLName("LocationTagEdge")]
public class LocationTagEdge
{
    [GraphQLName("node")] public LocationTagDetails Node { get; set; }

    [GraphQLName("cursor")] public string Cursor { get; set; }
}

public enum LocationTagOrderField
{
    Name,
    Description,
    TagType
}

[GraphQLName("LocationTagOrderInput")]
public class LocationTagOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }

    [GraphQLName("field")] public LocationTagOrderField Field { get; set; }
}

[GraphQLName("LocationTagPayload")]
public class LocationTagPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("locationTag")] public LocationTagDetails LocationTag { get; set; }
}

[GraphQLName("LocationTagWhereInput")]
public class LocationTagWhereInput
{
    [GraphQLName("locationId")] public string LocationId { get; set; }

    [GraphQLName("tagType")] public string? TagType { get; set; }

    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}

[GraphQLName("LocationWhereInput")]
public class LocationWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("nameContains")] public string? NameContains { get; set; }
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

[GraphQLName("RejectInvitationToJoinLocationInput")]
public class RejectInvitationToJoinLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }
}

[GraphQLName("RejectInvitationToJoinLocationPayload")]
public class RejectInvitationToJoinLocationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("UpdateDeskInput")]
public class UpdateDeskInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }

    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("deactivated")] public bool Deactivated { get; set; }

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("locationTagIds")] public string[] LocationTagIds { get; set; }
}

[GraphQLName("UpdateLocationInput")]
public class UpdateLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }

    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("about")] public string? About { get; set; }

    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("timezone")] public string? Timezone { get; set; }
}

[GraphQLName("UpdateLocationTagInput")]
public class UpdateLocationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }

    [GraphQLName("id")] public string Id { get; set; }

    [GraphQLName("name")] public string Name { get; set; }

    [GraphQLName("description")] public string? Description { get; set; }

    [GraphQLName("tagType")] public string TagType { get; set; }
}

[GraphQLName("Version")]
public class Version
{
    [GraphQLName("major")] public int Major { get; set; }

    [GraphQLName("minor")] public int Minor { get; set; }

    [GraphQLName("build")] public int Build { get; set; }

    [GraphQLName("revision")] public int Revision { get; set; }
}
