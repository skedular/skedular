using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("DeleteProductTagsInput")]
public class DeleteProductTagsInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("ids")]
    public IEnumerable<string> Ids { get; set; } = [];
}
