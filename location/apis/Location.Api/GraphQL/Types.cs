using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Relay;
using Location.Shared.Models;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Location.Api.GraphQL;

[GraphQLName("AcceptInvitationToJoinLocationInput")]
public class AcceptInvitationToJoinLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("AcceptInvitationToJoinLocationPayload")]
public class AcceptInvitationToJoinLocationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("AddLocationInput")]
public class AddLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("physicalAddress")] public LocationAddressDetails? PhysicalAddress { get; set; }
}

[GraphQLName("BulkAddDeskInput")]
public class BulkAddDeskInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("namePrefix")] public string? NamePrefix { get; set; }
    [GraphQLName("locationId")] public required string LocationId { get; set; }
    [GraphQLName("count")] public int Count { get; set; }
    [GraphQLName("deskTypeIds")] public string[] DeskTypeIds { get; set; } = [];
    [GraphQLName("zoneIds")] public string[] ZoneIds { get; set; } = [];
    [GraphQLName("deactivated")] public bool Deactivated { get; set; }

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }
}

[GraphQLName("BulkDeskPayload")]
public class BulkDeskPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("desks")] public DeskDetails[] Desks { get; set; } = [];
}

[GraphQLName("CancelInvitationToJoinLocationInput")]
public class CancelInvitationToJoinLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("CancelInvitationToJoinLocationPayload")]
public class CancelInvitationToJoinLocationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("ChangeLocationMemberRoleInput")]
public class ChangeLocationMemberRoleInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("role")] public LocationMemberRole Role { get; set; }
}

[GraphQLName("DeleteLocationInput")]
public class DeleteLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("DeskConnection")]
public class DeskConnection : Connection<DeskEdge>;

[GraphQLName("DeskDetails")]
public class DeskDetails : Node
{
    [GraphQLName("name")] public required string Name { get; set; }
    [GraphQLName("deactivated")] public bool Deactivated { get; set; }

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("deskTypes")] public OrganizationTagDetails[] DeskTypes { get; set; } = [];
    [GraphQLName("zones")] public OrganizationTagDetails[] Zones { get; set; } = [];
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("DeskEdge")]
public class DeskEdge : Edge<DeskDetails>;

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
    [GraphQLName("locationId")] public required string LocationId { get; set; }
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("zoneIds")] public string[]? ZoneIds { get; set; }
    [GraphQLName("deskTypeIds")] public string[]? DeskTypeIds { get; set; }
}

[GraphQLName("InviteCustomersToJoinLocationInput")]
public class InviteCustomersToJoinLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("locationId")] public required string LocationId { get; set; }
    [GraphQLName("emails")] public string[] Emails { get; set; } = [];
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
    public LocationDesksOccupancyPercentage[] DesksOccupancyPercentage { get; set; } = [];

    [GraphQLName("dailyBookingsTotals")] public LocationDailyBookingsTotal[] DailyBookingsTotals { get; set; } = [];
}

[GraphQLName("LocationConnection")]
public class LocationConnection : Connection<LocationEdge>;

[GraphQLName("LocationCustomerDetails")]
public class LocationCustomerDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
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
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("organization")] public LocationOrganizationDetails? Organization { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("deskCapacity")] public int DeskCapacity { get; set; }
    [GraphQLName("hasFutureBooking")] public bool HasFutureBooking { get; set; }
    [GraphQLName("canModify")] public bool CanModify { get; set; }
    [GraphQLName("canDelete")] public bool CanDelete { get; set; }
    [GraphQLName("canInvitePeople")] public bool CanInvitePeople { get; set; }
    [GraphQLName("canViewAnalytics")] public bool CanViewAnalytics { get; set; }
    [GraphQLName("desks")] public DeskDetails[] Desks { get; set; } = [];
    [GraphQLName("physicalAddress")] public LocationAddressDetails? PhysicalAddress { get; set; }
    [GraphQLName("deskTypes")] public OrganizationTagDetails[] DeskTypes { get; set; } = [];
    [GraphQLName("zones")] public OrganizationTagDetails[] Zones { get; set; } = [];
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("LocationEdge")]
public class LocationEdge : Edge<LocationDetails>;

[GraphQLName("LocationMemberConnection")]
public class LocationMemberConnection : Connection<LocationMemberEdge>;

[GraphQLName("LocationMemberDetails")]
public class LocationMemberDetails : Node
{
    [GraphQLName("role")] public LocationMemberRole? Role { get; set; }
    [GraphQLName("customer")] public LocationCustomerDetails Customer { get; set; }
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("LocationMemberDetailsPayload")]
public class LocationMemberDetailsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("member")] public LocationMemberDetails? Member { get; set; }
}

[GraphQLName("LocationMemberEdge")]
public class LocationMemberEdge : Edge<LocationMemberDetails>;

[GraphQLName("LocationMemberOrderInput")]
public class LocationMemberOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public LocationMemberOrderField Field { get; set; }
}

[GraphQLName("LocationMemberWhereInput")]
public class LocationMemberWhereInput
{
    [GraphQLName("locationId")] public required string LocationId { get; set; }
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
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
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }
}

[GraphQLName("LocationPayload")]
public class LocationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("location")] public LocationDetails Location { get; set; }
}

[GraphQLName("Organization_OrganizationTagDetails")]
public class OrganizationTagDetails
{
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("tagType")] public string? TagType { get; set; }
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
}

[GraphQLName("LocationWhereInput")]
public class LocationWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("zoneIds")] public string[]? ZoneIds { get; set; }
    [GraphQLName("deskTypeIds")] public string[]? DeskTypeIds { get; set; }
}

[GraphQLName("RejectInvitationToJoinLocationInput")]
public class RejectInvitationToJoinLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("RejectInvitationToJoinLocationPayload")]
public class RejectInvitationToJoinLocationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("UpdateLocationInput")]
public class UpdateLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("physicalAddress")] public LocationAddressDetails? PhysicalAddress { get; set; }
}

[GraphQLName("AddDeskInput")]
public class AddDeskInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("locationId")] public required string LocationId { get; set; }
    [GraphQLName("deskTypeIds")] public string[] DeskTypeIds { get; set; } = [];
    [GraphQLName("zoneIds")] public string[] ZoneIds { get; set; } = [];
}

[GraphQLName("UpdateDeskInput")]
public class UpdateDeskInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("deactivated")] public bool Deactivated { get; set; }

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("deskTypeIds")] public string[] DeskTypeIds { get; set; } = [];
    [GraphQLName("zoneIds")] public string[] ZoneIds { get; set; } = [];
}

[GraphQLName("DeleteDeskInput")]
public class DeleteDeskInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("LocationAddressDetails")]
public class LocationAddressDetails
{
    [GraphQLName("formattedAddress")] public string? FormattedAddress { get; set; }
    [GraphQLName("addressLine1")] public string? AddressLine1 { get; set; }
    [GraphQLName("addressLine2")] public string? AddressLine2 { get; set; }
    [GraphQLName("suburb")] public string? Suburb { get; set; }
    [GraphQLName("city")] public string? City { get; set; }
    [GraphQLName("province")] public string? Province { get; set; }
    [GraphQLName("zipcode")] public string? Zipcode { get; set; }
    [GraphQLName("country")] public string? Country { get; set; }
}
