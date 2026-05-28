using HotChocolate;

namespace Location.Api.GraphQL.Resource;

[GraphQLName("DeleteResourceInput")]
public class DeleteResourceInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}
