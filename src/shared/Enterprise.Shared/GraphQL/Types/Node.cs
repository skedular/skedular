namespace Enterprise.Shared.GraphQL.Types;

[GraphQLName("Node")]
public class Node(string? id = null)
{
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.IsNullOrWhiteSpace(id) ? string.Empty : id;
}
