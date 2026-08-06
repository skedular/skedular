using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("HostListingProductReadyProduct")]
public class HostListingProductReadyProductDetails
{
    [GraphQLName("id")]
    public string Id { get; init; } = string.Empty;

    [GraphQLName("inactive")]
    public bool Inactive { get; init; }
}
