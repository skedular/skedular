using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Pagination;
using HotChocolate.Types.Relay;
using Location.Shared.Models;

// ReSharper disable ClassNeverInstantiated.Global

namespace Location.Api.GraphQL;

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

    [GraphQLName("primaryFeatureImageUrl")]
    public string? PrimaryFeatureImageUrl { get; set; }
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

    [GraphQLName("primaryFeatureImageUrl")]
    public string? PrimaryFeatureImageUrl { get; set; }
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

    [GraphQLName("primaryFeatureImageUrl")]
    public string? PrimaryFeatureImageUrl { get; set; }

    [GraphQLName("floorPlans")] public IEnumerable<FloorPlanDetails> FloorPlans { get; set; } = [];
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

[GraphQLName("AddResourceInput")]
public class AddResourceInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("inactive")] public bool Inactive { get; set; }
    [GraphQLName("locationId")] public string LocationId { get; set; } = string.Empty;
    [GraphQLName("customTagIds")] public IEnumerable<string> CustomTagIds { get; set; } = [];
    [GraphQLName("zoneIds")] public IEnumerable<string> ZoneIds { get; set; } = [];
    [GraphQLName("productTagIds")] public IEnumerable<string> ProductTagIds { get; set; } = [];

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("color")] public string? Color { get; set; }
    [GraphQLName("capacity")] public int Capacity { get; set; }

    [GraphQLName("organizationResourceTypeId")]
    public string OrganizationResourceTypeId { get; set; } = string.Empty;
}

[GraphQLName("UpdateResourceInput")]
public class UpdateResourceInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
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
    public string OrganizationResourceTypeId { get; set; } = string.Empty;
}

[GraphQLName("DeleteResourceInput")]
public class DeleteResourceInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}

[GraphQLName("ActivateResourcesInput")]
public class ActivateResourcesInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
}

[GraphQLName("DeactivateResourcesInput")]
public class DeactivateResourcesInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
}

[GraphQLName("DeleteResourcesInput")]
public class DeleteResourcesInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
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
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("inactive")] public bool Inactive { get; set; }

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("color")] public string? Color { get; set; }
    [GraphQLName("capacity")] public int Capacity { get; set; }
    [GraphQLName("customTags")] public IEnumerable<OrganizationTagDetails> CustomTags { get; set; } = [];
    [GraphQLName("zones")] public IEnumerable<OrganizationTagDetails> Zones { get; set; } = [];
    [GraphQLName("productTags")] public IEnumerable<OrganizationTagDetails> ProductTags { get; set; } = [];
    [GraphQLName("resourceType")] public OrganizationTagDetails ResourceType { get; set; } = new();

    [GraphQLName("isAvailableHoursOverridden")]
    public bool IsAvailableHoursOverridden { get; set; }

    [GraphQLName("availableHours")] public OpeningHours? AvailableHours { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
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
    [GraphQLName("resource")] public ResourceDetails Resource { get; set; } = new();
}

[GraphQLName("ResourceWhereInput")]
public class ResourceWhereInput
{
    [GraphQLName("locationId")] public string LocationId { get; set; } = string.Empty;
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("customTagIds")] public IEnumerable<string>? CustomTagIds { get; set; }
    [GraphQLName("zoneIds")] public IEnumerable<string>? ZoneIds { get; set; }
    [GraphQLName("productTagIds")] public IEnumerable<string>? ProductTagIds { get; set; }
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

[GraphQLName("UpdateLocationResourceAvailableHoursInput")]
public class UpdateLocationResourceAvailableHoursInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;

    [GraphQLName("overrideAvailableHours")]
    public bool OverrideAvailableHours { get; set; }

    [GraphQLName("availableHours")] public WeekOpeningHours? AvailableHours { get; set; }
}

[GraphQLName("UpdateResourcePositionInput")]
public class UpdateResourcePositionInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("resourceId")] public string ResourceId { get; set; } = string.Empty;
    [GraphQLName("floorPlanId")] public string FloorPlanId { get; set; } = string.Empty;
    [GraphQLName("x")] public int X { get; set; }
    [GraphQLName("y")] public int Y { get; set; }
    [GraphQLName("width")] public int Width { get; set; }
    [GraphQLName("height")] public int Height { get; set; }
    [GraphQLName("shape")] public string? Shape { get; set; }
    [GraphQLName("metadata")] public string? Metadata { get; set; }
}

[GraphQLName("ResourcePosition")]
public class ResourcePosition
{
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
    [GraphQLName("x")] public int X { get; set; }
    [GraphQLName("y")] public int Y { get; set; }
    [GraphQLName("width")] public int Width { get; set; }
    [GraphQLName("height")] public int Height { get; set; }
    [GraphQLName("shape")] public string? Shape { get; set; }
    [GraphQLName("metadata")] public string? Metadata { get; set; }
    [GraphQLName("resource")] public ResourceDetails Resource { get; set; } = new();
}

[GraphQLName("ResourcePositionPayload")]
public class ResourcePositionPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("resourcePosition")] public ResourcePosition ResourcePosition { get; set; } = new();
}

[GraphQLName("RemoveResourcePositionInput")]
public class RemoveResourcePositionInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("resourceId")] public string ResourceId { get; set; } = string.Empty;
}

[GraphQLName("RemoveResourcePositionPayload")]
public class RemoveResourcePositionPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("success")] public bool Success { get; set; }
}

[GraphQLName("AddFloorPlanInput")]
public class AddFloorPlanInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("locationId")] public string LocationId { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("floorLevel")] public int FloorLevel { get; set; }
    [GraphQLName("floorName")] public string? FloorName { get; set; }
    [GraphQLName("imageBase64")] public string ImageBase64 { get; set; } = string.Empty;
    [GraphQLName("imageFileName")] public string ImageFileName { get; set; } = string.Empty;
}

[GraphQLName("UpdateFloorPlanInput")]
public class UpdateFloorPlanInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("floorLevel")] public int FloorLevel { get; set; }
    [GraphQLName("floorName")] public string? FloorName { get; set; }
    [GraphQLName("isActive")] public bool IsActive { get; set; }
}

[GraphQLName("DeleteFloorPlanInput")]
public class DeleteFloorPlanInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}

[GraphQLName("FloorPlanDetails")]
public class FloorPlanDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("floorLevel")] public int FloorLevel { get; set; }
    [GraphQLName("floorName")] public string? FloorName { get; set; }
    [GraphQLName("imagePath")] public string ImagePath { get; set; } = string.Empty;
    [GraphQLName("thumbnailPath")] public string? ThumbnailPath { get; set; }
    [GraphQLName("width")] public int Width { get; set; }
    [GraphQLName("height")] public int Height { get; set; }
    [GraphQLName("isActive")] public bool IsActive { get; set; }
    [GraphQLName("resourcePositions")] public IEnumerable<ResourcePosition> ResourcePositions { get; set; } = [];
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}

[GraphQLName("FloorPlanPayload")]
public class FloorPlanPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("floorPlan")] public FloorPlanDetails FloorPlan { get; set; } = new();
}

[GraphQLName("DeleteFloorPlanPayload")]
public class DeleteFloorPlanPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("success")] public bool Success { get; set; }
}

[GraphQLName("ResourcePositionInput")]
public class ResourcePositionInput
{
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("resourceId")] public string ResourceId { get; set; } = string.Empty;
    [GraphQLName("x")] public int X { get; set; }
    [GraphQLName("y")] public int Y { get; set; }
    [GraphQLName("width")] public int Width { get; set; }
    [GraphQLName("height")] public int Height { get; set; }
    [GraphQLName("shape")] public string? Shape { get; set; }
    [GraphQLName("metadata")] public string? Metadata { get; set; }
}

[GraphQLName("UpdateResourcePositionsInput")]
public class UpdateResourcePositionsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("floorPlanId")] public string FloorPlanId { get; set; } = string.Empty;
    [GraphQLName("positions")] public IEnumerable<ResourcePositionInput> Positions { get; set; } = [];
}

[GraphQLName("UpdateResourcePositionsPayload")]
public class UpdateResourcePositionsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("resourcePositions")] public IEnumerable<ResourcePosition> ResourcePositions { get; set; } = [];
}
