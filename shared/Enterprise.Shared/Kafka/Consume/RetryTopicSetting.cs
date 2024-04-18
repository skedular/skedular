namespace Enterprise.Shared.Kafka.Consume;

public class RetryTopicSetting
{
    public string Topic { get; set; } = string.Empty;
    public double RetryDelaySeconds { get; set; }
}
