namespace Enterprise.Shared.Temporal.Configurations;

public static class Defaults
{
    public const int CacheMaxInstances = 1000;
    public const int CapacityMaxConcurrentWorkflowTaskExecutors = 100;
    public const int CapacityMaxConcurrentActivityTaskExecutors = 100;
    public const int CapacityMaxConcurrentLocalActivityExecutors = 100;
    public const int CapacityMaxConcurrentWorkflowTaskPollers = 5;
    public const int CapacityMaxConcurrentActivityTaskPollers = 5;
}

public class TemporalConfiguration
{
    public const string Key = "Temporal";

    public WorkerConfig Worker { get; set; } = new();
    public ConnectionConfig Connection { get; set; } = new();
}

public record CapacityConfig(
    int MaxConcurrentWorkflowTaskPollers = Defaults.CapacityMaxConcurrentWorkflowTaskPollers,
    int MaxConcurrentWorkflowTaskExecutors = Defaults.CapacityMaxConcurrentWorkflowTaskExecutors,
    int MaxConcurrentActivityTaskPollers = Defaults.CapacityMaxConcurrentActivityTaskPollers,
    int MaxConcurrentLocalActivityExecutors = Defaults.CapacityMaxConcurrentLocalActivityExecutors,
    int MaxConcurrentActivityExecutors = Defaults.CapacityMaxConcurrentActivityTaskExecutors);

public record RateLimitsConfig(double? MaxWorkerActivitiesPerSecond = null, double? MaxTaskQueueActivitiesPerSecond = null);

public record CacheConfig(int MaxInstances = Defaults.CacheMaxInstances);

public record MtlsConfig(string KeyFile, string CertChainFile);

public class ConnectionConfig
{
    public string Namespace { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public MtlsConfig? Mtls { get; set; }
}

public class WorkerConfig
{
    public string TaskQueue { get; set; } = string.Empty;
    public CapacityConfig Capacity { get; set; } = new();
    public RateLimitsConfig RateLimits { get; set; } = new();
    public CacheConfig Cache { get; set; } = new();
}
