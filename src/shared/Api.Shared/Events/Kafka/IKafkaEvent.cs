namespace Api.Shared.Events.Kafka;

/// <summary>
///     Types implementing IEvent should be decorated with KafkaTopicAttribute.
/// </summary>
public interface IKafkaEvent
{
    string TopicName { get; }
    string RetryTopicNamePrefix { get; }
    int RetryTopicCount { get; }
    string DeadLetterTopicName { get; }
    string? CorrelationId { get; }
}
