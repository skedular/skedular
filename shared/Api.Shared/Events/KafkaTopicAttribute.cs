namespace Api.Shared.Events;

public class KafkaTopicAttribute(
    string topicName,
    int topicPartitionCount,
    string retryTopicNamePrefix,
    int retryTopicCount,
    int retryTopicPartitionCount,
    string deadLetterTopicName,
    int deadLetterTopicPartitionCount,
    string protobufSchema)
    : Attribute
{
    public string TopicName { get; } = topicName;
    public int TopicPartitionCount { get; } = topicPartitionCount;
    public string RetryTopicNamePrefix { get; } = retryTopicNamePrefix;
    public int RetryTopicCount { get; } = retryTopicCount;
    public int RetryTopicPartitionCount { get; } = retryTopicPartitionCount;
    public string DeadLetterTopicName { get; } = deadLetterTopicName;
    public int DeadLetterTopicPartitionCount { get; } = deadLetterTopicPartitionCount;
    public string ProtobufSchema { get; } = protobufSchema;
}
