using Api.Shared.Events.Kafka;

namespace Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1;

file static class CustomerReadinessMetadataShape
{
    internal const string TopicName = "customer_readiness.event";
    internal const string RetryTopicNamePrefix = "customer_readiness.event.retry";
    internal const int RetryTopicCount = 1;
    internal const string DeadLetterTopicName = "customer_readiness.event.deadletter";
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Key : IKafkaEvent
{
    string IKafkaEvent.TopicName => CustomerReadinessMetadataShape.TopicName;
    string IKafkaEvent.RetryTopicNamePrefix => CustomerReadinessMetadataShape.RetryTopicNamePrefix;
    int IKafkaEvent.RetryTopicCount => CustomerReadinessMetadataShape.RetryTopicCount;
    string IKafkaEvent.DeadLetterTopicName => CustomerReadinessMetadataShape.DeadLetterTopicName;
    string? IKafkaEvent.CorrelationId => null;
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Event : IKafkaEvent
{
    string IKafkaEvent.TopicName => CustomerReadinessMetadataShape.TopicName;
    string IKafkaEvent.RetryTopicNamePrefix => CustomerReadinessMetadataShape.RetryTopicNamePrefix;
    int IKafkaEvent.RetryTopicCount => CustomerReadinessMetadataShape.RetryTopicCount;
    string IKafkaEvent.DeadLetterTopicName => CustomerReadinessMetadataShape.DeadLetterTopicName;
    string? IKafkaEvent.CorrelationId => Metadata.CorrelationId;

    public static Metadata NewMetadata(
        string domainSource,
        string appSource,
        Type type,
        string? correlationId,
        Guid? id = null) =>
        KafkaEventMetadataFactory.NewMetadata<Metadata, Type>(domainSource, appSource, type, correlationId, id);
}

public sealed partial class Metadata : IKafkaEventMetadata<Type>;
