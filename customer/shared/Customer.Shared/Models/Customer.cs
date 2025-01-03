using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class Customer : ModelBaseWithDeleted
{
    public string? Designation { get; set; }
    public string? Title { get; set; }
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
    public string? Timezone { get; set; }
    public string? Locale { get; set; }
    public string? PhoneNumber { get; set; }

    public bool? IsOrganizationOnboardingDone { get; set; }
    public bool? IsLocationOnboardingDone { get; set; }
    public bool? IsTeamOnboardingDone { get; set; }
    public bool? IsDefaultOrganizationOnboardingDone { get; set; }
    public bool? IsDefaultLocationOnboardingDone { get; set; }
    public bool? IsPreferredZoneOnboardingDone { get; set; }
    public bool? IsPreferredDeskOnboardingDone { get; set; }

    public ICollection<Identity> Identities { get; set; } = [];
    public ICollection<CustomerFeedback> CustomerFeedbacks { get; set; } = [];
    public Organization? DefaultOrganization { get; set; }
    public ICollection<Location> DefaultLocations { get; set; } = [];
    public ICollection<Desk> PreferredDesks { get; set; } = [];
    public ICollection<Team> DefaultTeams { get; set; } = [];
    public ICollection<OrganizationTag> PreferredOrganizationTags { get; set; } = [];
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public ICollection<LocationMember> LocationMembers { get; set; } = [];
    public ICollection<TeamMember> TeamMembers { get; set; } = [];
}
