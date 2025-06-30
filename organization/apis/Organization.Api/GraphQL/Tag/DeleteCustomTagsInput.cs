using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("DeleteCustomTagsInput")]
public class DeleteCustomTagsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("ids")] public IEnumerable<string> Ids { get; set; } = [];
}
