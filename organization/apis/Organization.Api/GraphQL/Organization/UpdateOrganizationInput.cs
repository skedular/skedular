using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("UpdateOrganizationInput")]
public class UpdateOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }

    [GraphQLName("uniqueAlphanumericName")]
    public string? UniqueAlphanumericName { get; set; }

    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("website")] public string? Website { get; set; }
    [GraphQLName("contactEmail")] public string? ContactEmail { get; set; }
    [GraphQLName("contactPhone")] public string? ContactPhone { get; set; }

    [GraphQLName("industrySubCategoryIds")]
    public IEnumerable<string> IndustrySubCategoryIds { get; set; } = [];

    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile>? FeatureImages { get; set; } = [];
    [GraphQLName("listingMetadata")] public ListingMetadata? ListingMetadata { get; set; } = ListingMetadata.Empty();
}
