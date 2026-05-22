using Api.Shared.Services.Models;
using HotChocolate;
using Location.Shared.Models;

namespace Location.Api.Models;

[GraphQLName("LocationPatchField")]
public enum LocationPatchField
{
    Name,
    Timezone,
    Type,
    Organization,
    Tags,
    FeatureImages,
    ExtraMetadata,
    ListingMetadata,
    PhysicalAddress,
    UniqueClaimCode
}

[GraphQLName("LocationOpeningHoursPatchField")]
public enum LocationOpeningHoursPatchField
{
    WeekOpeningHours
}

[GraphQLName("LocationRestrictedInformationPatchField")]
public enum LocationRestrictedInformationPatchField
{
    Title,
    Category,
    Content,
    Active,
    SortOrder
}

[GraphQLName("LocationPhysicalAddressPatchField")]
public enum LocationPhysicalAddressPatchField
{
    Address
}

[GraphQLName("FloorPlanPatchField")]
public enum FloorPlanPatchField
{
    Name,
    Image,
    ResourcePositions
}

[GraphQLName("ResourcePositionsPatchField")]
public enum ResourcePositionsPatchField
{
    ResourcePositions
}

[GraphQLName("ResourcePatchField")]
public enum ResourcePatchField
{
    Name,
    Inactive,
    RequireBookingApproval,
    Color,
    Capacity,
    Tags,
    ResourceType
}

[GraphQLName("LocationResourceAvailableHoursPatchField")]
public enum LocationResourceAvailableHoursPatchField
{
    AvailableHours
}

public record LocationPatchRequest(
    Shared.Models.Location Location,
    IReadOnlySet<LocationPatchField> FieldsToUpdate);

public record ResourcePatchRequest(
    Resource Resource,
    IReadOnlySet<ResourcePatchField> FieldsToUpdate);

public record LocationOpeningHoursPatchRequest(
    string Id,
    WeekOpeningHours WeekOpeningHours,
    IReadOnlySet<LocationOpeningHoursPatchField> FieldsToUpdate);

public record LocationRestrictedInformationPatchRequest(
    LocationRestrictedInformation RestrictedInformation,
    IReadOnlySet<LocationRestrictedInformationPatchField> FieldsToUpdate);

public record LocationPhysicalAddressPatchRequest(
    LocationPhysicalAddress PhysicalAddress,
    IReadOnlySet<LocationPhysicalAddressPatchField> FieldsToUpdate);

public record FloorPlanPatchRequest(
    FloorPlan FloorPlan,
    IReadOnlySet<FloorPlanPatchField> FieldsToUpdate);

public record ResourcePositionsPatchRequest(
    string FloorPlanId,
    IReadOnlyList<ResourcePosition> ResourcePositions,
    IReadOnlySet<ResourcePositionsPatchField> FieldsToUpdate);

public record ResourceAvailableHoursPatchRequest(
    string Id,
    bool OverrideAvailableHours,
    WeekOpeningHours? AvailableHours,
    IReadOnlySet<LocationResourceAvailableHoursPatchField> FieldsToUpdate);
