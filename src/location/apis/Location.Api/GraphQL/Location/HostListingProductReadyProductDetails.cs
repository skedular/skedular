using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("HostListingProductReadyProduct")]
public class HostListingProductReadyProductDetails
{
    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;

    [GraphQLName("inactive")]
    public bool Inactive { get; set; }
}
