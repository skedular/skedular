namespace Enterprise.Shared.GraphQL.Configurations;

public class GraphqlConfig
{
    public const string Key = "GraphQL";

    public bool IncludeCookies { get; set; }
    public bool NitroEnabled { get; set; }
    public bool DisableTelemetry { get; set; }
    public bool IntrospectionEnabled { get; set; }
    public bool CollectOperationPlanTelemetry { get; set; }
    public bool AllowErrorHandlingModeOverride { get; set; }
    public TimeSpan? ExecutionTimeout { get; set; }
    public TimeSpan? SubgraphAttemptTimeout { get; set; }
    public bool IncludeExceptionDetails { get; set; }
    public string Path { get; set; } = string.Empty;
    public IReadOnlyList<string> WarmupQueries { get; set; } = [];
}
