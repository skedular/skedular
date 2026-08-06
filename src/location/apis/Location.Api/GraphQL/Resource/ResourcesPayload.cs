using HotChocolate;

namespace Location.Api.GraphQL.Resource;

[GraphQLName("ResourcesPayload")]
public class ResourcesPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("resources")]
    public IEnumerable<ResourceDetails> Resources { get; set; } = [];
}
