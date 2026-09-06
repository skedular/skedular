using Api.Shared.Events.Kafka;

namespace Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1;

file static class OrganizationInternalMetadataShape
{
    internal const string TopicName = "organization.v1.internal";
    internal const string RetryTopicNamePrefix = "organization.v1.internal.retry";
    internal const int RetryTopicCount = 1;
    internal const string DeadLetterTopicName = "organization.v1.internal.deadletter";
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Key : IKafkaEvent
{
    string IKafkaEvent.TopicName => OrganizationInternalMetadataShape.TopicName;
    string IKafkaEvent.RetryTopicNamePrefix => OrganizationInternalMetadataShape.RetryTopicNamePrefix;
    int IKafkaEvent.RetryTopicCount => OrganizationInternalMetadataShape.RetryTopicCount;
    string IKafkaEvent.DeadLetterTopicName => OrganizationInternalMetadataShape.DeadLetterTopicName;
    string? IKafkaEvent.CorrelationId => null;
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Event : IKafkaEvent
{
    string IKafkaEvent.TopicName => OrganizationInternalMetadataShape.TopicName;
    string IKafkaEvent.RetryTopicNamePrefix => OrganizationInternalMetadataShape.RetryTopicNamePrefix;
    int IKafkaEvent.RetryTopicCount => OrganizationInternalMetadataShape.RetryTopicCount;
    string IKafkaEvent.DeadLetterTopicName => OrganizationInternalMetadataShape.DeadLetterTopicName;
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
