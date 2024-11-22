namespace Enterprise.Shared.GraphQL.Types;

[GraphQLName("Node")]
public interface Node
{
    [GraphQLName("id")] [ID] public string Id { get; set; }
}
