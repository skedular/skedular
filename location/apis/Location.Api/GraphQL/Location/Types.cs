using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Pagination;
using HotChocolate.Types.Relay;
using Location.Api.GraphQL.Resource;
using Location.Shared.Models;

// ReSharper disable ClassNeverInstantiated.Global

namespace Location.Api.GraphQL.Location;

[GraphQLName("AddLocationInput")]
public class AddLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("physicalAddress")] public AddressDetailsInput PhysicalAddress { get; set; } = new();
    [GraphQLName("locationTagIds")] public IEnumerable<string> LocationTagIds { get; set; } = [];
    [GraphQLName("contactEmail")] public string? ContactEmail { get; set; }
    [GraphQLName("contactPhone")] public string? ContactPhone { get; set; }
    [GraphQLName("primaryFeatureImage")] public CdnImageFile? PrimaryFeatureImage { get; set; }
}

[GraphQLName("UpdateLocationInput")]
public class UpdateLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("physicalAddress")] public AddressDetailsInput PhysicalAddress { get; set; } = new();
    [GraphQLName("locationTagIds")] public IEnumerable<string> LocationTagIds { get; set; } = [];
    [GraphQLName("contactEmail")] public string? ContactEmail { get; set; }
    [GraphQLName("contactPhone")] public string? ContactPhone { get; set; }
    [GraphQLName("primaryFeatureImage")] public CdnImageFile? PrimaryFeatureImage { get; set; }
}

[GraphQLName("DeleteLocationInput")]
public class DeleteLocationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}

[GraphQLName("LocationAnalytics")]
public class LocationAnalytics
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;

    [GraphQLName("desksOccupancyPercentage")]
    public IEnumerable<DesksOccupancyPercentage> DesksOccupancyPercentage { get; set; } = [];

    [GraphQLName("roomsOccupancyPercentage")]
    public IEnumerable<RoomsOccupancyPercentage> RoomsOccupancyPercentage { get; set; } = [];

    [GraphQLName("dailyBookingsTotals")] public IEnumerable<LocationDailyBookingsTotal> DailyBookingsTotals { get; set; } = [];
}

[GraphQLName("LocationConnection")]
public class LocationConnection : Enterprise.Shared.GraphQL.Types.Connection<LocationEdge>;

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
    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; } = new();
    [GraphQLName("timezone")] public string? Timezone { get; set; }
    [GraphQLName("openingHours")] public OpeningHours OpeningHours { get; set; } = new();
    [GraphQLName("deskCapacity")] public int DeskCapacity { get; set; }
    [GraphQLName("roomCapacity")] public int RoomCapacity { get; set; }
    [GraphQLName("hasFutureBooking")] public bool HasFutureBooking { get; set; }
    [GraphQLName("canModify")] public bool CanModify { get; set; }
    [GraphQLName("canDelete")] public bool CanDelete { get; set; }
    [GraphQLName("canViewAnalytics")] public bool CanViewAnalytics { get; set; }
    [GraphQLName("resources")] public IEnumerable<ResourceDetails> Resources { get; set; } = [];
    [GraphQLName("physicalAddress")] public AddressDetails PhysicalAddress { get; set; } = new();
    [GraphQLName("customTags")] public IEnumerable<OrganizationTagDetails> CustomTags { get; set; } = [];
    [GraphQLName("zones")] public IEnumerable<OrganizationTagDetails> Zones { get; set; } = [];
    [GraphQLName("resourceTypes")] public IEnumerable<OrganizationTagDetails> ResourceTypes { get; set; } = [];
    [GraphQLName("locationTags")] public IEnumerable<OrganizationTagDetails> LocationTags { get; set; } = [];
    [GraphQLName("contactEmail")] public string? ContactEmail { get; set; }
    [GraphQLName("contactPhone")] public string? ContactPhone { get; set; }
    [GraphQLName("primaryFeatureImage")] public CdnImageFile? PrimaryFeatureImage { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
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
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }
}

[GraphQLName("LocationPayload")]
public class LocationPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("location")] public LocationDetails Location { get; set; } = new();
}

[GraphQLName("Location_OrganizationTagDetails")]
public class OrganizationTagDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
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

[GraphQLName("LocationAddressDetails")]
public class AddressDetails
{
    [GraphQLName("formattedAddress")] public string? FormattedAddress { get; set; }
    [GraphQLName("addressLine1")] public string AddressLine1 { get; set; } = string.Empty;
    [GraphQLName("addressLine2")] public string? AddressLine2 { get; set; }
    [GraphQLName("suburb")] public string Suburb { get; set; } = string.Empty;
    [GraphQLName("city")] public string City { get; set; } = string.Empty;
    [GraphQLName("province")] public string? Province { get; set; }
    [GraphQLName("zipcode")] public string Zipcode { get; set; } = string.Empty;
    [GraphQLName("country")] public string Country { get; set; } = string.Empty;
}

[GraphQLName("LocationAddressDetailsInput")]
public class AddressDetailsInput
{
    [GraphQLName("addressLine1")] public string AddressLine1 { get; set; } = string.Empty;
    [GraphQLName("addressLine2")] public string? AddressLine2 { get; set; }
    [GraphQLName("suburb")] public string Suburb { get; set; } = string.Empty;
    [GraphQLName("city")] public string City { get; set; } = string.Empty;
    [GraphQLName("province")] public string? Province { get; set; }
    [GraphQLName("zipcode")] public string Zipcode { get; set; } = string.Empty;
    [GraphQLName("country")] public string Country { get; set; } = string.Empty;
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
    [GraphQLName("openingHoursDetails")] public OpeningHoursDetails OpeningHoursDetails { get; set; } = new();
}

[GraphQLName("WeekOpeningHours")]
public class WeekOpeningHours
{
    [GraphQLName("monday")] public OpeningHoursDetails Monday { get; set; } = new();
    [GraphQLName("tuesday")] public OpeningHoursDetails Tuesday { get; set; } = new();
    [GraphQLName("wednesday")] public OpeningHoursDetails Wednesday { get; set; } = new();
    [GraphQLName("thursday")] public OpeningHoursDetails Thursday { get; set; } = new();
    [GraphQLName("friday")] public OpeningHoursDetails Friday { get; set; } = new();
    [GraphQLName("saturday")] public OpeningHoursDetails Saturday { get; set; } = new();
    [GraphQLName("sunday")] public OpeningHoursDetails Sunday { get; set; } = new();
}

[GraphQLName("OpeningHours")]
public class OpeningHours
{
    [GraphQLName("weekOpeningHours")] public WeekOpeningHours WeekOpeningHours { get; set; } = new();
    [GraphQLName("closedDates")] public IEnumerable<DateTimeOffset> ClosedDates { get; set; } = [];

    [GraphQLName("datesWithVariedOpeningHours")]
    public IEnumerable<VariedDateOpeningHours> DatesWithVariedOpeningHours { get; set; } = [];
}

[GraphQLName("UpdateLocationOpeningHoursInput")]
public class UpdateLocationOpeningHoursInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("weekOpeningHours")] public WeekOpeningHours WeekOpeningHours { get; set; } = new();
}
