using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ActivateProductsInput")]
public class ActivateProductsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
}
