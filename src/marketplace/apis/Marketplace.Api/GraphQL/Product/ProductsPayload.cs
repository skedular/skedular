using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductsPayload")]
public class ProductsPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("products")]
    public IEnumerable<ProductDetails> Products { get; set; } = [];
}
