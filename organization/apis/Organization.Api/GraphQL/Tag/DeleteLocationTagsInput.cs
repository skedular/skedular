using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("DeleteLocationTagsInput")]
public class DeleteLocationTagsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
}
