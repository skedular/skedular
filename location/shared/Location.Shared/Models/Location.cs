using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class Location : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public string? Timezone { get; set; }
    public LocationType Type { get; set; }
    public OpeningHours? OpeningHours { get; set; }
    public ICollection<CdnImageFile> FeatureImages { get; set; } = [];
    public LocationExtraMetadata? ExtraMetadata { get; set; }
    public string? UniqueClaimCode { get; set; }
    public bool ContactedViaEmail { get; set; }
    public bool ContactedViaSms { get; set; }
    public bool ContactedViaCall { get; set; }
    public bool ContactedViaWhatsapp { get; set; }
    public ListingMetadata ListingMetadata { get; set; } = ListingMetadata.Empty();

    public Organization Organization { get; set; } = new();
    public ICollection<Resource> Resources { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<DailyDeskCountRecording> DailyDeskCountRecordings { get; set; } = [];
    public ICollection<DailyRoomCountRecording> DailyRoomCountRecordings { get; set; } = [];
    public ICollection<OrganizationTag> Tags { get; set; } = [];

    public ICollection<OrganizationTag> CustomTags =>
        Resources
            .SelectMany(item => item.Tags.Where(tag => tag.Type == OrganizationTagType.Custom).Select(customTag =>
                new OrganizationTag { Id = customTag.Id, Type = OrganizationTagType.Custom }))
            .GroupBy(item => item.Id)
            .Select(group => group.First())
            .ToList();

    public ICollection<OrganizationTag> Zones =>
        Resources
            .SelectMany(item => item.Tags.Where(tag => tag.Type == OrganizationTagType.Zone).Select(customTag =>
                new OrganizationTag { Id = customTag.Id, Type = OrganizationTagType.Zone }))
            .GroupBy(item => item.Id)
            .Select(group => group.First())
            .ToList();

    public ICollection<OrganizationTag> SpaceTypes =>
        Tags.Where(item => OrganizationTagTypeConstants.LocationSpaceTypes.Any(tagType => item.Type == tagType)).ToList();

    public ICollection<OrganizationTag> Amenities =>
        Tags.Where(item => OrganizationTagTypeConstants.Amenities.Any(tagType => item.Type == tagType)).ToList();

    public ICollection<Booking> InvolvedBookings { get; set; } = [];
    public ICollection<FloorPlan> FloorPlans { get; set; } = [];
    public LocationPhysicalAddress? PhysicalAddress { get; set; }
    public Permissions Permissions { get; set; } = new();
    public ICollection<PrecomputedLocationProduct> PrecomputedLocationProducts { get; set; } = [];
}
