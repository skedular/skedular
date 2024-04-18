namespace Enterprise.Shared.GraphQL.Configurations;

public class GraphqlConfig
{
    public const string Key = "GraphQL";

    public bool ClientEnabled { get; set; }
    public bool IntrospectionEnabled { get; set; }
    public string Path { get; set; } = string.Empty;
}
