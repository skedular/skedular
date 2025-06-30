using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("DeleteLocationTagInput")]
public class DeleteLocationTagInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}
