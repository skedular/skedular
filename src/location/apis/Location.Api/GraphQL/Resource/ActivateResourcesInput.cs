using HotChocolate;

namespace Location.Api.GraphQL.Resource;

[GraphQLName("ActivateResourcesInput")]
public class ActivateResourcesInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
}
