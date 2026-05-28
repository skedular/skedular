namespace Api.Shared.Events;

public class KafkaTopicAttribute(
    int topicPartitionCount,
    int retryTopicCount,
    int retryTopicPartitionCount,
    int deadLetterTopicPartitionCount)
    : Attribute
{
    public int TopicPartitionCount { get; } = topicPartitionCount;
    public int RetryTopicCount { get; } = retryTopicCount;
    public int RetryTopicPartitionCount { get; } = retryTopicPartitionCount;
    public int DeadLetterTopicPartitionCount { get; } = deadLetterTopicPartitionCount;
}
