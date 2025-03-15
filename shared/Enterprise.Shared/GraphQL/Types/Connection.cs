namespace Enterprise.Shared.GraphQL.Types;

public class Connection<T> where T : class
{
    [GraphQLName("pageInfo")] public PageInfo PageInfo { get; set; } = new();

    [GraphQLName("edges")] public IEnumerable<T> Edges { get; set; } = [];

    [GraphQLName("totalCount")] public int? TotalCount { get; set; }
}
