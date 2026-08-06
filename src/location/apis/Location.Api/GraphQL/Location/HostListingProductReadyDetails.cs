using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("HostListingProductReady")]
public class HostListingProductReadyDetails
{
    [GraphQLName("locationId")]
    public string LocationId { get; init; } = string.Empty;

    [GraphQLName("product")]
    public HostListingProductReadyProductDetails? Product { get; init; }
}
