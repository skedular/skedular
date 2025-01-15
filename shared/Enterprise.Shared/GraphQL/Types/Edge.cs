namespace Enterprise.Shared.GraphQL.Types;

public class Edge<T> where T : Node
{
    [GraphQLName("node")] public T Node { get; set; }
    [GraphQLName("cursor")] public string Cursor { get; set; } = string.Empty;
}
