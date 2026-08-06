using HotChocolate;

namespace Location.Api.GraphQL.Resource;

[GraphQLName("BulkAddResourcesPayload")]
public class BulkAddResourcesPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("results")]
    public IEnumerable<BulkAddResourceRowResult> Results { get; set; } = [];
}
