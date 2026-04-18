using Api.Shared.Events;

namespace Api.Shared.Clients.Events.Skedular.Team.V1;

file static class TeamMetadataShape
{
    internal const string TopicName = "team.v1.event";
    internal const string RetryTopicNamePrefix = "team.v1.event.retry";
    internal const int RetryTopicCount = 1;
    internal const string DeadLetterTopicName = "team.v1.event.deadletter";
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Key : IEvent
{
    string IEvent.TopicName => TeamMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => TeamMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => TeamMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => TeamMetadataShape.DeadLetterTopicName;
    string? IEvent.CorrelationId => null;
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Event : IEvent
{
    string IEvent.TopicName => TeamMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => TeamMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => TeamMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => TeamMetadataShape.DeadLetterTopicName;
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
