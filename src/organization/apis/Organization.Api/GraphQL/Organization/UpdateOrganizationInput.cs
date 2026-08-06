using Api.Shared.Services.Models;
using HotChocolate;
using Organization.Api.GraphQL.PhysicalAddress;
using Organization.Api.Models;

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("UpdateOrganizationInput")]
public class UpdateOrganizationInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("id")]
    public string? Id { get; set; }

    [GraphQLName("customDomain")]
    public string? CustomDomain { get; set; }

    [GraphQLName("fieldsToUpdate")]
    public IEnumerable<OrganizationPatchField> FieldsToUpdate { get; set; } = [];

    [GraphQLName("name")]
    public string? Name { get; set; }

    [GraphQLName("website")]
    public string? Website { get; set; }

    [GraphQLName("logoUrl")]
    public string? LogoUrl { get; set; }

    [GraphQLName("customerFacingTermsAndConditionsUrl")]
    public string? CustomerFacingTermsAndConditionsUrl { get; set; }

    [GraphQLName("billingCycle")]
    public OrganizationBillingCycle? BillingCycle { get; set; }

    [GraphQLName("invoiceDueInDays")]
    public int? InvoiceDueInDays { get; set; }

    [GraphQLName("contactEmail")]
    public string? ContactEmail { get; set; }

    [GraphQLName("contactPhone")]
    public string? ContactPhone { get; set; }

    [GraphQLName("refundNotificationEmails")]
    public IEnumerable<string>? RefundNotificationEmails { get; set; }

    [GraphQLName("industrySubCategoryIds")]
    public IEnumerable<string>? IndustrySubCategoryIds { get; set; }

    [GraphQLName("featureImages")]
    public IEnumerable<CdnImageFile>? FeatureImages { get; set; }

    [GraphQLName("marketplaceListingMetadata")]
    public ListingMetadata? MarketplaceListingMetadata { get; set; }

    [GraphQLName("physicalAddress")]
    public OrganizationPhysicalAddressPatchInput? PhysicalAddress { get; set; }
}
