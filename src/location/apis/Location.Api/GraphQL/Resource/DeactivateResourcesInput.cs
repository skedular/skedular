using HotChocolate;

namespace Location.Api.GraphQL.Resource;

[GraphQLName("DeactivateResourcesInput")]
public class DeactivateResourcesInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
}
