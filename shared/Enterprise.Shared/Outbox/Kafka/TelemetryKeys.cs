namespace Enterprise.Shared.Outbox.Kafka;

public static class TelemetryKeys
{
    public const string KafkaActivitySourceName = "kafka_outbox";
    public const string KafkaEventPoll = "kafka outbox poll";
    public const string KafkaEventSave = "kafka outbox save";
    public const string KafkaEventSend = "kafka outbox send";
}
