using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("UpdateOrganizationMarketplaceListingMetadataInput")]
public class UpdateOrganizationMarketplaceListingMetadataInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }

    [GraphQLName("uniqueAlphanumericName")]
    public string? UniqueAlphanumericName { get; set; }

    [GraphQLName("marketplaceListingMetadata")]
    public ListingMetadata MarketplaceListingMetadata { get; set; } = ListingMetadata.Empty;
}
