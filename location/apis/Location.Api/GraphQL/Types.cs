using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Pagination;
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
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("physicalAddress")] public LocationAddressDetails? PhysicalAddress { get; set; }
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

[GraphQLName("DeleteLocationInput")]
public class DeleteLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("LocationAnalytics")]
public class LocationAnalytics
{
    [GraphQLName("name")] public required string Name { get; set; }

    [GraphQLName("desksOccupancyPercentage")]
    public IEnumerable<DesksOccupancyPercentage> DesksOccupancyPercentage { get; set; } = [];

    [GraphQLName("roomsOccupancyPercentage")]
    public IEnumerable<RoomsOccupancyPercentage> RoomsOccupancyPercentage { get; set; } = [];

    [GraphQLName("dailyBookingsTotals")] public IEnumerable<LocationDailyBookingsTotal> DailyBookingsTotals { get; set; } = [];
}

[GraphQLName("LocationConnection")]
public class LocationConnection : Enterprise.Shared.GraphQL.Types.Connection<LocationEdge>;

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

[GraphQLName("LocationDetails")]
public class LocationDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("openingHours")] public OpeningHours OpeningHours { get; set; }
    [GraphQLName("deskCapacity")] public int DeskCapacity { get; set; }
    [GraphQLName("roomCapacity")] public int RoomCapacity { get; set; }
    [GraphQLName("hasFutureBooking")] public bool HasFutureBooking { get; set; }
    [GraphQLName("canModify")] public bool CanModify { get; set; }
    [GraphQLName("canDelete")] public bool CanDelete { get; set; }
    [GraphQLName("canViewAnalytics")] public bool CanViewAnalytics { get; set; }
    [GraphQLName("resources")] public IEnumerable<ResourceDetails> Resources { get; set; } = [];
    [GraphQLName("physicalAddress")] public LocationAddressDetails? PhysicalAddress { get; set; }
    [GraphQLName("customTags")] public IEnumerable<OrganizationTagDetails> CustomTags { get; set; } = [];
    [GraphQLName("zones")] public IEnumerable<OrganizationTagDetails> Zones { get; set; } = [];
    [GraphQLName("resourceTypes")] public IEnumerable<OrganizationTagDetails> ResourceTypes { get; set; } = [];
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("LocationEdge")]
public class LocationEdge(LocationDetails node, string cursor) : Edge<LocationDetails>(node, cursor);

[GraphQLName("LocationOrderInput")]
public class LocationOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public LocationOrderField Field { get; set; }
}

[GraphQLName("Location_OrganizationDetails")]
public class OrganizationDetails
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

[GraphQLName("Location_OrganizationTagDetails")]
public class OrganizationTagDetails
{
    [GraphQLName("uniqueId")] [ID] public required string UniqueId { get; set; }
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("tagType")] public string? TagType { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}

[GraphQLName("LocationWhereInput")]
public class LocationWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
    [GraphQLName("locationIds")] public IEnumerable<string>? LocationIds { get; set; } = [];
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("customTagIds")] public IEnumerable<string>? CustomTagIds { get; set; }
    [GraphQLName("zoneIds")] public IEnumerable<string>? ZoneIds { get; set; }
}

[GraphQLName("UpdateLocationInput")]
public class UpdateLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("physicalAddress")] public LocationAddressDetails? PhysicalAddress { get; set; }
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

[GraphQLName("DesksOccupancyPercentage")]
public class DesksOccupancyPercentage
{
    [GraphQLName("date")] public DateTimeOffset Date { get; set; }
    [GraphQLName("percentage")] public float Percentage { get; set; }
}

[GraphQLName("RoomsOccupancyPercentage")]
public class RoomsOccupancyPercentage
{
    [GraphQLName("date")] public DateTimeOffset Date { get; set; }
    [GraphQLName("percentage")] public float Percentage { get; set; }
}

[GraphQLName("AddResourceInput")]
public class AddResourceInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("inactive")] public bool Inactive { get; set; }
    [GraphQLName("locationId")] public required string LocationId { get; set; }
    [GraphQLName("customTagIds")] public IEnumerable<string> CustomTagIds { get; set; } = [];
    [GraphQLName("zoneIds")] public IEnumerable<string> ZoneIds { get; set; } = [];
    [GraphQLName("productTagIds")] public IEnumerable<string> ProductTagIds { get; set; } = [];

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("color")] public string? Color { get; set; }
    [GraphQLName("capacity")] public int Capacity { get; set; }

    [GraphQLName("organizationResourceTypeId")]
    public required string OrganizationResourceTypeId { get; set; }
}

[GraphQLName("UpdateResourceInput")]
public class UpdateResourceInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("inactive")] public bool Inactive { get; set; }

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("color")] public string? Color { get; set; }
    [GraphQLName("capacity")] public int Capacity { get; set; }
    [GraphQLName("customTagIds")] public IEnumerable<string> CustomTagIds { get; set; } = [];
    [GraphQLName("zoneIds")] public IEnumerable<string> ZoneIds { get; set; } = [];
    [GraphQLName("productTagIds")] public IEnumerable<string> ProductTagIds { get; set; } = [];

    [GraphQLName("organizationResourceTypeId")]
    public required string OrganizationResourceTypeId { get; set; }
}

[GraphQLName("DeleteResourceInput")]
public class DeleteResourceInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}

[GraphQLName("ActivateResourcesInput")]
public class ActivateResourcesInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public required IEnumerable<string> Ids { get; set; }
}

[GraphQLName("DeactivateResourcesInput")]
public class DeactivateResourcesInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public required IEnumerable<string> Ids { get; set; }
}

[GraphQLName("DeleteResourcesInput")]
public class DeleteResourcesInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public required IEnumerable<string> Ids { get; set; }
}

[GraphQLName("ResourcesPayload")]
public class ResourcesPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("resources")] public IEnumerable<ResourceDetails> Resources { get; set; } = [];
}

[GraphQLName("ResourceConnection")]
public class ResourceConnection : Enterprise.Shared.GraphQL.Types.Connection<ResourceEdge>;

[GraphQLName("ResourceDetails")]
public class ResourceDetails : Node
{
    [GraphQLName("name")] public required string Name { get; set; }
    [GraphQLName("inactive")] public bool Inactive { get; set; }

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("color")] public string? Color { get; set; }
    [GraphQLName("capacity")] public int Capacity { get; set; }
    [GraphQLName("customTags")] public IEnumerable<OrganizationTagDetails> CustomTags { get; set; } = [];
    [GraphQLName("zones")] public IEnumerable<OrganizationTagDetails> Zones { get; set; } = [];
    [GraphQLName("productTags")] public IEnumerable<OrganizationTagDetails> ProductTags { get; set; } = [];
    [GraphQLName("resourceType")] public OrganizationTagDetails ResourceType { get; set; }

    [GraphQLName("isAvailableHoursOverridden")]
    public bool IsAvailableHoursOverridden { get; set; }

    [GraphQLName("availableHours")] public OpeningHours? AvailableHours { get; set; }
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}

[GraphQLName("ResourceEdge")]
public class ResourceEdge(ResourceDetails node, string cursor) : Edge<ResourceDetails>(node, cursor);

[GraphQLName("ResourceOrderInput")]
public class ResourceOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public ResourceOrderField Field { get; set; }
}

[GraphQLName("ResourcePayload")]
public class ResourcePayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("resource")] public ResourceDetails Resource { get; set; }
}

[GraphQLName("ResourceWhereInput")]
public class ResourceWhereInput
{
    [GraphQLName("locationId")] public required string LocationId { get; set; }
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("customTagIds")] public IEnumerable<string>? CustomTagIds { get; set; }
    [GraphQLName("zoneIds")] public IEnumerable<string>? ZoneIds { get; set; }
    [GraphQLName("productTagIds")] public IEnumerable<string>? ProductTagIds { get; set; }
}

[GraphQLName("ResourcesOccupancyPercentage")]
public class ResourcesOccupancyPercentage
{
    [GraphQLName("date")] public DateTimeOffset Date { get; set; }
    [GraphQLName("percentage")] public float Percentage { get; set; }
}

[GraphQLName("OpeningHoursDetails")]
public class OpeningHoursDetails
{
    [GraphQLName("closed")] public bool Closed { get; set; }
    [GraphQLName("openAllDay")] public bool OpenAllDay { get; set; }
    [GraphQLName("from")] public string? From { get; set; }
    [GraphQLName("until")] public string? Until { get; set; }
}

[GraphQLName("VariedDateOpeningHours")]
public class VariedDateOpeningHours
{
    [GraphQLName("date")] public DateTimeOffset Date { get; set; }
    [GraphQLName("openingHoursDetails")] public OpeningHoursDetails OpeningHoursDetails { get; set; }
}

[GraphQLName("WeekOpeningHours")]
public class WeekOpeningHours
{
    [GraphQLName("monday")] public OpeningHoursDetails Monday { get; set; }
    [GraphQLName("tuesday")] public OpeningHoursDetails Tuesday { get; set; }
    [GraphQLName("wednesday")] public OpeningHoursDetails Wednesday { get; set; }
    [GraphQLName("thursday")] public OpeningHoursDetails Thursday { get; set; }
    [GraphQLName("friday")] public OpeningHoursDetails Friday { get; set; }
    [GraphQLName("saturday")] public OpeningHoursDetails Saturday { get; set; }
    [GraphQLName("sunday")] public OpeningHoursDetails Sunday { get; set; }
}

[GraphQLName("OpeningHours")]
public class OpeningHours
{
    [GraphQLName("weekOpeningHours")] public WeekOpeningHours WeekOpeningHours { get; set; }
    [GraphQLName("closedDates")] public IEnumerable<DateTimeOffset> ClosedDates { get; set; }

    [GraphQLName("datesWithVariedOpeningHours")]
    public IEnumerable<VariedDateOpeningHours> DatesWithVariedOpeningHours { get; set; }
}

[GraphQLName("UpdateLocationOpeningHoursInput")]
public class UpdateLocationOpeningHoursInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
    [GraphQLName("weekOpeningHours")] public WeekOpeningHours WeekOpeningHours { get; set; }
}

[GraphQLName("UpdateLocationResourceAvailableHoursInput")]
public class UpdateLocationResourceAvailableHoursInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }

    [GraphQLName("overrideAvailableHours")]
    public bool OverrideAvailableHours { get; set; }

    [GraphQLName("availableHours")] public WeekOpeningHours? AvailableHours { get; set; }
}
