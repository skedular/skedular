using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("DeleteProductTagInput")]
public class DeleteProductTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}
