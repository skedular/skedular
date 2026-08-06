using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductPayload")]
public class ProductPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("product")]
    public ProductDetails Product { get; set; } = new();
}
