namespace Enterprise.Shared.Telemetry.Configurations;

public class OpenTelemetryConfiguration
{
    public const string Key = "OpenTelemetry";

    public bool ConsoleEnabled { get; set; }
    public bool MetricsIngestEnabled { get; set; }
    public bool EntityFrameworkEnabled { get; set; }
    public string MeterProviderName { get; set; } = string.Empty;
}
