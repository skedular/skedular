namespace Enterprise.Shared.GraphQL.Types;

[GraphQLName("Node")]
public interface Node
{
    [GraphQLName("id")] [ID] string Id { get; set; }
}
