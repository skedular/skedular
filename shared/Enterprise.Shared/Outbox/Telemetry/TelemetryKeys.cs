namespace Enterprise.Shared.Outbox.Telemetry;

public static class TelemetryKeys
{
    public const string ActivitySourceName = "kafka_outbox";
    public const string EventSave = "outbox save";
    public const string EventSend = "outbox send";
}
