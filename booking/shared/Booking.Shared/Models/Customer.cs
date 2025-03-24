using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Customer : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? GivenName { get; set; }
    public string? MiddleName { get; set; }
    public string? FamilyName { get; set; }
    public string? PhotoUrl { get; set; }
    public string? PhotoUrl24 { get; set; }
    public string? PhotoUrl32 { get; set; }
    public string? PhotoUrl48 { get; set; }
    public string? PhotoUrl72 { get; set; }
    public string? PhotoUrl192 { get; set; }
    public string? PhotoUrl512 { get; set; }

    public ICollection<Identity> Identities { get; set; } = [];
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public ICollection<LocationMember> LocationMembers { get; set; } = [];
    public ICollection<TeamMember> TeamMembers { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
    public Organization? DefaultOrganization { get; set; }
    public ICollection<Location> PreferredLocations { get; set; } = [];
    public ICollection<Resource> PreferredResources { get; set; } = [];
    public ICollection<Team> PreferredTeams { get; set; } = [];
    public ICollection<OrganizationTag> PreferredOrganizationTags { get; set; } = [];
    public ICollection<ResourceBookingSlot> ResourceBookingSlots { get; set; } = [];
}
