using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class Location : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public string? Timezone { get; set; }
    public LocationType Type { get; set; }
    public OpeningHours? OpeningHours { get; set; }
    public IReadOnlyList<CdnImageFile> FeatureImages { get; set; } = [];
    public LocationExtraMetadata? ExtraMetadata { get; set; }
    public string? UniqueClaimCode { get; set; }
    public bool ContactedViaEmail { get; set; }
    public bool ContactedViaSms { get; set; }
    public bool ContactedViaCall { get; set; }
    public bool ContactedViaWhatsapp { get; set; }
    public ListingMetadata ListingMetadata { get; set; } = ListingMetadata.Empty;

    public Organization Organization { get; set; } = new();
    public IReadOnlyList<Resource> Resources { get; set; } = [];
    public IReadOnlyList<DailyDeskCountRecording> DailyDeskCountRecordings { get; set; } = [];
    public IReadOnlyList<DailyRoomCountRecording> DailyRoomCountRecordings { get; set; } = [];
    public IReadOnlyList<OrganizationTag> OrganizationTags { get; set; } = [];

    public IReadOnlyList<OrganizationTag> CustomTags =>
    [
        .. Resources
            .SelectMany(item => item.Tags.Where(tag => tag.Type == OrganizationTagType.Custom))
            .GroupBy(item => item.Id)
            .Select(group => group.First()),
    ];

    public IReadOnlyList<OrganizationTag> Zones =>
    [
        .. Resources.SelectMany(item => item.Tags.Where(tag => tag.Type == OrganizationTagType.Zone))
            .GroupBy(item => item.Id)
            .Select(group => group.First()),
    ];

    public IReadOnlyList<OrganizationTag> SpaceTypes =>
        [.. OrganizationTags.Where(item => OrganizationTagTypeConstants.LocationSpaceTypes.Any(tagType => item.Type == tagType))];

    public IReadOnlyList<OrganizationTag> Amenities =>
        [.. OrganizationTags.Where(item => OrganizationTagTypeConstants.Amenities.Any(tagType => item.Type == tagType))];

    public IReadOnlyList<FloorPlan> FloorPlans { get; set; } = [];
    public LocationPhysicalAddress? PhysicalAddress { get; set; }
    public Permissions Permissions { get; set; } = new();
    public IReadOnlyList<PrecomputedLocationProduct> PrecomputedLocationProducts { get; set; } = [];
    public IReadOnlyList<LocationRestrictedInformation> RestrictedInformation { get; set; } = [];
}
