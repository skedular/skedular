namespace Enterprise.Shared.Telemetry.Configurations;

public class OpenTelemetrySettings
{
    public const string Key = "OpenTelemetry";

    public bool ConsoleEnabled { get; set; }
    public bool MetricsIngestEnabled { get; set; }
    public string ZipkinEndpoint { get; set; } = string.Empty;
    public string JaegerEndpoint { get; set; } = string.Empty;
    public bool EntityFrameworkEnabled { get; set; }
}
