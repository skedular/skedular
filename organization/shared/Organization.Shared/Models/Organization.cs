using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class Organization : ModelBaseWithDeleted
{
    public string? CustomDomain { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? CustomerFacingTermsAndConditionsUrl { get; set; }
    public bool AgreedToTermsOfUse { get; set; }
    public string? LogoUrl { get; set; }
    public OrganizationType Type { get; set; }
    public bool HasAttachedPaymentMethod => OrganizationStripePaymentMethods.Count != 0;
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public IReadOnlyList<string> RefundNotificationEmails { get; set; } = [];
    public bool? IsOwnershipVerified { get; set; }
    public Uri StripeAuthorizeExistingConnectAccountUrl { get; set; } = Constants.EmptyUri;
    public IReadOnlyList<CdnImageFile> FeatureImages { get; set; } = [];
    public ListingMetadata ListingMetadata { get; set; } = ListingMetadata.Empty;
    public ListingMetadata MarketplaceListingMetadata { get; set; } = ListingMetadata.Empty;

    public IReadOnlyList<OrganizationMember> OrganizationMembers { get; set; } = [];
    public TermsOfUse? TermsOfUse { get; set; }
    public IReadOnlyList<OrganizationOffering> OrganizationOfferings { get; set; } = [];
    public IReadOnlyList<DailyMemberCountRecording> DailyMemberCountRecordings { get; set; } = [];
    public IReadOnlyList<IndustrySubCategory> IndustrySubCategories { get; set; } = [];
    public IReadOnlyList<JoinInvitation> JoinInvitations { get; set; } = [];
    public IReadOnlyList<AzureTenant> AzureTenants { get; set; } = [];
    public IReadOnlyList<Tag> Tags { get; set; } = [];
    public OrganizationSsoSettings? OrganizationSsoSettings { get; set; }
    public IReadOnlyList<OrganizationStripePaymentMethod> OrganizationStripePaymentMethods { get; set; } = [];
    public OrganizationStripeCustomer? OrganizationStripeCustomer { get; set; }
    public OrganizationBillingDetails? BillingDetails { get; set; }
    public IReadOnlyList<OrganizationStripeConnectAccount> OrganizationStripeConnectAccounts { get; set; } = [];
    public IReadOnlyList<OrganizationBankAccount> OrganizationBankAccounts { get; set; } = [];
    public OrganizationTaxDetails? OrganizationTaxDetails { get; set; }
    public OrganizationXeroConnection? OrganizationXeroConnection { get; set; }
    public OrganizationPhysicalAddress? PhysicalAddress { get; set; }

    public bool CanModify { get; set; }
    public bool CanDelete { get; set; }
    public bool CanInvitePeople { get; set; }
    public bool CanViewAnalytics { get; set; }

    public bool IsMyOnboardingDone { get; set; }

    public OrganizationBillingCycle BillingCycle { get; set; }
    public int InvoiceDueInDays { get; set; }
}
