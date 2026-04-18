using Api.Shared.Events;

namespace Api.Shared.Clients.Events.Skedular.Location.V1;

file static class LocationMetadataShape
{
    internal const string TopicName = "location.v1.event";
    internal const string RetryTopicNamePrefix = "location.v1.event.retry";
    internal const int RetryTopicCount = 1;
    internal const string DeadLetterTopicName = "location.v1.event.deadletter";
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Key : IEvent
{
    string IEvent.TopicName => LocationMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => LocationMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => LocationMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => LocationMetadataShape.DeadLetterTopicName;
    string? IEvent.CorrelationId => null;
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Event : IEvent
{
    string IEvent.TopicName => LocationMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => LocationMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => LocationMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => LocationMetadataShape.DeadLetterTopicName;
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
