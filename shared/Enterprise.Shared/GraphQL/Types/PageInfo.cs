namespace Enterprise.Shared.GraphQL.Types;

[GraphQLName("PageInfo")]
[Shareable]
public class PageInfo
{
    [GraphQLName("hasNextPage")] public bool HasNextPage { get; set; }
    [GraphQLName("hasPreviousPage")] public bool HasPreviousPage { get; set; }
    [GraphQLName("startCursor")] public string? StartCursor { get; set; }
    [GraphQLName("endCursor")] public string? EndCursor { get; set; }
}
