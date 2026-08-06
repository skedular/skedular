using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("DeactivateProductsInput")]
public class DeactivateProductsInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("ids")]
    public IEnumerable<string> Ids { get; set; } = [];
}
