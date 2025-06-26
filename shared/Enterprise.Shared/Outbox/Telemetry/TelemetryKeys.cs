namespace Enterprise.Shared.Outbox.Telemetry;

public static class TelemetryKeys
{
    public const string KafkaActivitySourceName = "kafka_outbox";
    public const string KafkaEventSave = "kafka outbox save";
    public const string KafkaEventSend = "kafka outbox send";

    public const string TemporalActivitySourceName = "temporal_outbox";
    public const string TemporalEventSave = "temporal outbox save";
    public const string TemporalEventSend = "temporal outbox send";
    
    public const string TemporalSignalActivitySourceName = "temporal_outbox";
    public const string TemporalSignalEventSave = "temporal outbox save";
    public const string TemporalSignalEventSend = "temporal outbox send";
}
