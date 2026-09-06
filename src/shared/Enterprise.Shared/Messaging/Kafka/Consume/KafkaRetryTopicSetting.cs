namespace Enterprise.Shared.Messaging.Kafka.Consume;

public class KafkaRetryTopicSetting
{
    public string Topic { get; set; } = string.Empty;
    public double RetryDelaySeconds { get; set; }
}
