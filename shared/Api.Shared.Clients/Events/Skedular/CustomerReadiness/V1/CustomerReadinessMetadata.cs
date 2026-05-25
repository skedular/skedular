using Api.Shared.Events;

namespace Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1;

file static class CustomerReadinessMetadataShape
{
    internal const string TopicName = "customer_readiness";
    internal const string RetryTopicNamePrefix = "customer_readiness.retry";
    internal const int RetryTopicCount = 1;
    internal const string DeadLetterTopicName = "customer_readiness.deadletter";
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Key : IEvent
{
    string IEvent.TopicName => CustomerReadinessMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => CustomerReadinessMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => CustomerReadinessMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => CustomerReadinessMetadataShape.DeadLetterTopicName;
    string? IEvent.CorrelationId => null;
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Event : IEvent
{
    string IEvent.TopicName => CustomerReadinessMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => CustomerReadinessMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => CustomerReadinessMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => CustomerReadinessMetadataShape.DeadLetterTopicName;
    string? IEvent.CorrelationId => Metadata.CorrelationId;

    public static Metadata NewMetadata(
        string domainSource,
        string appSource,
        Type type,
        string? correlationId,
        Guid? id = null) =>
        EventMetadataFactory.NewMetadata<Metadata, Type>(domainSource, appSource, type, correlationId, id);
}

public sealed partial class Metadata : IEventMetadata<Type>;
