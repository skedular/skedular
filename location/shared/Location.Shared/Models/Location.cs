using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class Location : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public string? About { get; set; }
    public string? Timezone { get; set; }
    public DateTimeOffset? DailyDeskCountLastRecordedAt { get; set; }

    public Address? PhysicalAddress { get; set; }

    public Organization? Organization { get; set; }
    public ICollection<Desk> Desks { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<LocationMember> LocationMembers { get; set; } = [];
    public ICollection<JoinInvitation> JoinInvitations { get; set; } = [];
    public ICollection<DailyDeskCountRecording> DailyDeskCountRecordings { get; set; } = [];

    public ICollection<OrganizationTag> CustomTags { get; set; } = [];
    public ICollection<OrganizationTag> Zones { get; set; } = [];

    public bool HasFutureBooking { get; set; }
    public Permissions Permissions { get; set; } = new();
}
