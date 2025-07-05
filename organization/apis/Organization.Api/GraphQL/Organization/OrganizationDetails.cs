using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;
using Organization.Api.GraphQL.Billing;
using Organization.Api.GraphQL.Member;
using Organization.Api.GraphQL.Offering;
using Organization.Api.GraphQL.Sso;
using Organization.Api.GraphQL.Tag;
using Organization.Api.GraphQL.TaxDetails;

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("OrganizationDetails")]
public class OrganizationDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("website")] public string? Website { get; set; }
    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }
    [GraphQLName("type")] public OrganizationTypeDetails Type { get; set; } = new();

    [GraphQLName("memberVisibilityPolicy")]
    public OrganizationMemberVisibilityPolicyDetails MemberVisibilityPolicy { get; set; } = new();

    [GraphQLName("agreedToTermsOfUse")] public bool AgreedToTermsOfUse { get; set; }
    [GraphQLName("termsOfUse")] public OrganizationTermsOfUse? TermsOfUse { get; set; }

    [GraphQLName("industrySubCategories")]
    public IEnumerable<OrganizationIndustrySubCategoryReferenceDetails> IndustrySubCategories { get; set; } = [];

    [GraphQLName("contactEmail")] public string? ContactEmail { get; set; }
    [GraphQLName("contactPhone")] public string? ContactPhone { get; set; }

    [GraphQLName("physicalAddress")] public AddressDetails PhysicalAddress { get; set; } = new();
    [GraphQLName("billingDetails")] public OrganizationBillingDetails? BillingDetails { get; set; }

    [GraphQLName("availableOfferings")] public IEnumerable<OrganizationOfferingDetails> AvailableOfferings { get; set; } = [];
    [GraphQLName("activeOffering")] public OrganizationActiveOfferingDetails ActiveOffering { get; set; } = new();
    [GraphQLName("hasFutureBooking")] public bool HasFutureBooking { get; set; }
    [GraphQLName("hasLocation")] public bool HasLocation { get; set; }
    [GraphQLName("hasTeam")] public bool HasTeam { get; set; }
    [GraphQLName("canModify")] public bool CanModify { get; set; }
    [GraphQLName("canDelete")] public bool CanDelete { get; set; }
    [GraphQLName("canInvitePeople")] public bool CanInvitePeople { get; set; }
    [GraphQLName("canViewAnalytics")] public bool CanViewAnalytics { get; set; }
    [GraphQLName("isMyOnboardingDone")] public bool IsMyOnboardingDone { get; set; }
    [GraphQLName("members")] public IEnumerable<OrganizationMemberDetails> Members { get; set; } = [];
    [GraphQLName("resourceTypes")] public IEnumerable<OrganizationTagDetails> ResourceTypes { get; set; } = [];
    [GraphQLName("paymentMethods")] public IEnumerable<OrganizationPaymentMethod> PaymentMethods { get; set; } = [];

    [GraphQLName("hasAttachedPaymentMethod")]
    public bool HasAttachedPaymentMethod { get; set; }

    [GraphQLName("ssoSettings")] public OrganizationSsoSettingsDetails? SsoSettings { get; set; }
    [GraphQLName("taxDetails")] public OrganizationTaxDetails? TaxDetails { get; set; }

    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
