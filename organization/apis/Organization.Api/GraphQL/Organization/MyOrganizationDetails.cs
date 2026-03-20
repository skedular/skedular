using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("MyOrganizationDetails")]
public class MyOrganizationDetails
{
    [GraphQLName("uniqueId")] public string Id { get; set; } = string.Empty;
    [GraphQLName("customDomain")] public string? CustomDomain { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("website")] public string? Website { get; set; }

    [GraphQLName("customerFacingTermsAndConditionsUrl")]
    public string? CustomerFacingTermsAndConditionsUrl { get; set; }

    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }
    [GraphQLName("type")] public OrganizationTypeDetails Type { get; set; } = new();
    [GraphQLName("contactEmail")] public string? ContactEmail { get; set; }
    [GraphQLName("contactPhone")] public string? ContactPhone { get; set; }
    [GraphQLName("isMyOnboardingDone")] public bool IsMyOnboardingDone { get; set; }
    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile> FeatureImages { get; set; } = [];
    [GraphQLName("listingMetadata")] public ListingMetadata ListingMetadata { get; set; } = ListingMetadata.Empty;
}
