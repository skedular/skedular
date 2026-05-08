using Api.Shared.Services.Models;
using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("AddOrganizationInput")]
public class AddOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("customDomain")] public string? CustomDomain { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("website")] public string? Website { get; set; }
    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }

    [GraphQLName("customerFacingTermsAndConditionsUrl")]
    public string? CustomerFacingTermsAndConditionsUrl { get; set; }

    [GraphQLName("type")] public OrganizationType Type { get; set; }
    [GraphQLName("billingCycle")] public OrganizationBillingCycle BillingCycle { get; set; }
    [GraphQLName("invoiceDueInDays")] public int InvoiceDueInDays { get; set; }
    [GraphQLName("contactEmail")] public string? ContactEmail { get; set; }
    [GraphQLName("contactPhone")] public string? ContactPhone { get; set; }

    [GraphQLName("refundNotificationEmails")]
    public IEnumerable<string> RefundNotificationEmails { get; set; } = [];

    [GraphQLName("agreedToTermsOfUse")] public bool AgreedToTermsOfUse { get; set; }
    [GraphQLName("termsOfUseId")] public string TermsOfUseId { get; set; } = string.Empty;

    [GraphQLName("industrySubCategoryIds")]
    public IEnumerable<string> IndustrySubCategoryIds { get; set; } = [];

    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile>? FeatureImages { get; set; } = [];
    [GraphQLName("listingMetadata")] public ListingMetadata? ListingMetadata { get; set; } = ListingMetadata.Empty;

    [GraphQLName("marketplaceListingMetadata")]
    public ListingMetadata? MarketplaceListingMetadata { get; set; } = ListingMetadata.Empty;
}
