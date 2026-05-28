using Api.Shared.Events;

namespace Api.Shared.Clients.Events.Skedular.Customer.V1;

file static class CustomerMetadataShape
{
    internal const string TopicName = "customer.v1.event";
    internal const string RetryTopicNamePrefix = "customer.v1.event.retry";
    internal const int RetryTopicCount = 1;
    internal const string DeadLetterTopicName = "customer.v1.event.deadletter";
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Key : IEvent
{
    string IEvent.TopicName => CustomerMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => CustomerMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => CustomerMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => CustomerMetadataShape.DeadLetterTopicName;
    string? IEvent.CorrelationId => null;
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Event : IEvent
{
    string IEvent.TopicName => CustomerMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => CustomerMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => CustomerMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => CustomerMetadataShape.DeadLetterTopicName;
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
