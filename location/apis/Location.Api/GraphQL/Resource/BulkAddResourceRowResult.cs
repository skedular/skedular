using HotChocolate;

namespace Location.Api.GraphQL.Resource;

[GraphQLName("BulkAddResourceRowResult")]
public class BulkAddResourceRowResult
{
    [GraphQLName("rowIndex")] public int RowIndex { get; set; }
    [GraphQLName("createdResources")] public IEnumerable<ResourceDetails> CreatedResources { get; set; } = [];
    [GraphQLName("failureReason")] public string? FailureReason { get; set; }
}
