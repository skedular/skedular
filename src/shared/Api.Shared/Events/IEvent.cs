namespace Api.Shared.Events;

/// <summary>
///     Types implementing IEvent should be decorated with KafkaTopicAttribute.
/// </summary>
public interface IEvent
{
    string TopicName { get; }
    string RetryTopicNamePrefix { get; }
    int RetryTopicCount { get; }
    string DeadLetterTopicName { get; }
    string? CorrelationId { get; }
}
