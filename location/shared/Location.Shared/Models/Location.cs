using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class Location : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public string? About { get; set; }
    public string? Timezone { get; set; }
    public LocationType Type { get; set; }
    public OpeningHours? OpeningHours { get; set; }
    public CdnImageFile? PrimaryFeatureImage { get; set; }
    public LocationExtraMetadata? ExtraMetadata { get; set; }
    public string? UniqueClaimCode { get; set; }
    public bool ContactedViaEmail { get; set; }
    public bool ContactedViaSms { get; set; }
    public bool ContactedViaCall { get; set; }
    public bool ContactedViaWhatsapp { get; set; }

    public Organization Organization { get; set; } = new();
    public ICollection<Resource> Resources { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<DailyDeskCountRecording> DailyDeskCountRecordings { get; set; } = [];
    public ICollection<DailyRoomCountRecording> DailyRoomCountRecordings { get; set; } = [];
    public ICollection<OrganizationTag> CustomTags { get; set; } = [];
    public ICollection<OrganizationTag> Zones { get; set; } = [];
    public ICollection<OrganizationTag> Tags { get; set; } = [];
    public ICollection<Booking> InvolvedBookings { get; set; } = [];
    public ICollection<FloorPlan> FloorPlans { get; set; } = [];
    public LocationPhysicalAddress? PhysicalAddress { get; set; }
    public Permissions Permissions { get; set; } = new();
    public ICollection<PrecomputedLocationProduct> PrecomputedLocationProducts { get; set; } = [];
}
