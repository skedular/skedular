using HotChocolate;

namespace Location.Api.GraphQL.Resource;

[GraphQLName("ResourcePayload")]
public class ResourcePayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("resource")] public ResourceDetails Resource { get; set; } = new();
}
