namespace Enterprise.Shared.GraphQL.Configurations;

public class GraphqlConfig
{
    public const string Key = "GraphQL";

    public bool IncludeCookies { get; set; }
    public bool NitroEnabled { get; set; }
    public bool DisableTelemetry { get; set; }
    public bool IntrospectionEnabled { get; set; }
    public bool AllowQueryPlan { get; set; }
    public bool IncludeDebugInfo { get; set; }
    public bool IncludeExceptionDetails { get; set; }
    public string Path { get; set; } = string.Empty;
}
