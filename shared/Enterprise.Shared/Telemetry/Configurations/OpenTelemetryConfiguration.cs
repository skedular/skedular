namespace Enterprise.Shared.Telemetry.Configurations;

public class OpenTelemetryConfiguration
{
    public const string Key = "OpenTelemetry";

    public bool ConsoleEnabled { get; set; }
    public bool MetricsIngestEnabled { get; set; }
    public bool EntityFrameworkEnabled { get; set; }
    public string MeterProviderName { get; set; } = string.Empty;

    /// <summary>
    ///     When true, spans from outbox background services (kafka_outbox, temporal_outbox)
    ///     are suppressed. Useful in production to reduce telemetry volume from polling loops.
    /// </summary>
    public bool ExcludeOutboxTelemetry { get; set; }
}
