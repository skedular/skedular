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
    public OrganizationBillingCycle BillingCycle { get; set; }
    public bool HasAttachedPaymentMethod => OrganizationStripePaymentMethods.Count != 0;
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public bool? IsOwnershipVerified { get; set; }
    public Uri StripeAuthorizeExistingConnectAccountUrl { get; set; } = Constants.EmptyUri;
    public ICollection<CdnImageFile> FeatureImages { get; set; } = [];
    public ListingMetadata ListingMetadata { get; set; } = ListingMetadata.Empty;
    public ListingMetadata MarketplaceListingMetadata { get; set; } = ListingMetadata.Empty;

    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public TermsOfUse? TermsOfUse { get; set; }
    public ICollection<OrganizationOffering> OrganizationOfferings { get; set; } = [];
    public ICollection<DailyMemberCountRecording> DailyMemberCountRecordings { get; set; } = [];
    public ICollection<IndustrySubCategory> IndustrySubCategories { get; set; } = [];
    public ICollection<JoinInvitation> JoinInvitations { get; set; } = [];
    public ICollection<AzureTenant> AzureTenants { get; set; } = [];
    public ICollection<Tag> Tags { get; set; } = [];
    public OrganizationSsoSettings? OrganizationSsoSettings { get; set; }
    public ICollection<OrganizationStripePaymentMethod> OrganizationStripePaymentMethods { get; set; } = [];
    public OrganizationStripeCustomer? OrganizationStripeCustomer { get; set; }
    public OrganizationBillingDetails? BillingDetails { get; set; }
    public ICollection<OrganizationStripeConnectAccount> OrganizationStripeConnectAccounts { get; set; } = [];
    public ICollection<OrganizationBankAccount> OrganizationBankAccounts { get; set; } = [];
    public OrganizationTaxDetails? OrganizationTaxDetails { get; set; }
    public OrganizationXeroConnection? OrganizationXeroConnection { get; set; }
    public OrganizationPhysicalAddress? PhysicalAddress { get; set; }

    public bool CanModify { get; set; }
    public bool CanDelete { get; set; }
    public bool CanInvitePeople { get; set; }
    public bool CanViewAnalytics { get; set; }

    public bool IsMyOnboardingDone { get; set; }
}
