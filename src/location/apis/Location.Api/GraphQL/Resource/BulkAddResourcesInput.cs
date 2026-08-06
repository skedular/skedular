using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Location.Api.GraphQL.Resource;

[GraphQLName("BulkAddResourcesInput")]
public class BulkAddResourcesInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("locationId")]
    public string LocationId { get; set; } = string.Empty;

    [GraphQLName("rows")]
    public IReadOnlyList<BulkAddResourceRowInput> Rows { get; set; } = [];
}
