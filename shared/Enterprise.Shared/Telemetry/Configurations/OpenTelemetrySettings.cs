namespace Enterprise.Shared.Telemetry.Configurations;

public class OpenTelemetrySettings
{
    public const string Key = "OpenTelemetry";

    public bool ConsoleEnabled { get; set; }
    public bool ZipkinEnabled { get; set; }
    public string ZipkinEndpoint { get; set; } = string.Empty;
    public bool OtlpEnabled { get; set; }
    public string OtlpEndpoint { get; set; } = string.Empty;
    public bool MetricsIngestEnabled { get; set; }
    public bool JaegerEnabled { get; set; }
    public string JaegerEndpoint { get; set; } = string.Empty;
}
