using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("DeleteProductsInput")]
public class DeleteProductsInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("ids")]
    public IEnumerable<string> Ids { get; set; } = [];
}
