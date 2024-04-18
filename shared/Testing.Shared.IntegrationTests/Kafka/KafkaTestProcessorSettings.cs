namespace Testing.Shared.IntegrationTests.Kafka;

public class KafkaTestProcessorSettings
{
    public TimeSpan MaxWaitTimeUntilConsumerGroupIsSetup { get; set; }
    public string OutgoingTopicPrefix { get; set; } = string.Empty;
    public string IncomingTopicPrefix { get; set; } = string.Empty;
}
