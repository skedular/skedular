using Enterprise.Shared.Models;

namespace Team.Shared.Models;

public class Team : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public string? About { get; set; }
    public string? Timezone { get; set; }

    public Organization? Organization { get; set; }
    public Location? PrimaryLocation { get; set; }
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<TeamMember> TeamMembers { get; set; } = [];
    public ICollection<JoinInvitation> JoinInvitations { get; set; } = [];

    public bool HasFutureBooking { get; set; }
    public Permissions Permissions { get; set; } = new();
}
