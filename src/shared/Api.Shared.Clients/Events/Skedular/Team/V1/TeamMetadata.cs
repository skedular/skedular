using Api.Shared.Events.Kafka;

namespace Api.Shared.Clients.Events.Skedular.Team.V1;

file static class TeamMetadataShape
{
    internal const string TopicName = "team.v1.event";
    internal const string RetryTopicNamePrefix = "team.v1.event.retry";
    internal const int RetryTopicCount = 1;
    internal const string DeadLetterTopicName = "team.v1.event.deadletter";
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Key : IKafkaEvent
{
    string IKafkaEvent.TopicName => TeamMetadataShape.TopicName;
    string IKafkaEvent.RetryTopicNamePrefix => TeamMetadataShape.RetryTopicNamePrefix;
    int IKafkaEvent.RetryTopicCount => TeamMetadataShape.RetryTopicCount;
    string IKafkaEvent.DeadLetterTopicName => TeamMetadataShape.DeadLetterTopicName;
    string? IKafkaEvent.CorrelationId => null;
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Event : IKafkaEvent
{
    string IKafkaEvent.TopicName => TeamMetadataShape.TopicName;
    string IKafkaEvent.RetryTopicNamePrefix => TeamMetadataShape.RetryTopicNamePrefix;
    int IKafkaEvent.RetryTopicCount => TeamMetadataShape.RetryTopicCount;
    string IKafkaEvent.DeadLetterTopicName => TeamMetadataShape.DeadLetterTopicName;
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
