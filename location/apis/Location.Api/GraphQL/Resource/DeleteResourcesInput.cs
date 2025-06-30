using HotChocolate;

namespace Location.Api.GraphQL.Resource;

[GraphQLName("DeleteResourcesInput")]
public class DeleteResourcesInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
}
