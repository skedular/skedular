using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class Organization : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public string? About { get; set; }
    public string? Website { get; set; }
    public bool AgreedToTermsOfUse { get; set; }
    public string? LogoUrl { get; set; }
    public OrganizationType Type { get; set; }
    public OrganizationMemberVisibilityPolicy MemberVisibilityPolicy { get; set; }
    public bool HasAttachedPaymentMethod => StripePaymentMethods.Count != 0;
    public DateTimeOffset? PaymentMethodEventRaisedAt { get; set; }
    public DateTimeOffset? DailyMemberCountLastRecordedAt { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    public Address? PhysicalAddress { get; set; }
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public TermsOfUse? TermsOfUse { get; set; }
    public ICollection<OrganizationOffering> OrganizationOfferings { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<DailyMemberCountRecording> DailyMemberCountRecordings { get; set; } = [];
    public ICollection<IndustrySubCategory> IndustrySubCategories { get; set; } = [];
    public ICollection<Location> Locations { get; set; } = [];
    public ICollection<Team> Teams { get; set; } = [];
    public ICollection<JoinInvitation> JoinInvitations { get; set; } = [];
    public ICollection<AzureTenant> AzureTenants { get; set; } = [];
    public ICollection<Tag> Tags { get; set; } = [];
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
    public ICollection<Booking> InvolvedBookings { get; set; } = [];
    public ICollection<StripePaymentMethod> StripePaymentMethods { get; set; } = [];
    public StripeCustomer? StripeCustomer { get; set; }
    public OrganizationBillingDetails? OrganizationBillingDetails { get; set; }

    public bool HasFutureBooking { get; set; }
    public bool HasLocation { get; set; }
    public bool HasTeam { get; set; }
    public bool CanModify { get; set; }
    public bool CanDelete { get; set; }
    public bool CanInvitePeople { get; set; }
    public bool CanViewAnalytics { get; set; }

    public bool IsMyOnboardingDone { get; set; }
}
