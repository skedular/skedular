using Api.Shared.Events.Kafka;

namespace Api.Shared.Clients.Events.Skedular.OrganizationMember.V1;

file static class OrganizationMemberMetadataShape
{
    internal const string TopicName = "organization.member.v1.event";
    internal const string RetryTopicNamePrefix = "organization.member.v1.event.retry";
    internal const int RetryTopicCount = 1;
    internal const string DeadLetterTopicName = "organization.member.v1.event.deadletter";
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Key : IKafkaEvent
{
    string IKafkaEvent.TopicName => OrganizationMemberMetadataShape.TopicName;
    string IKafkaEvent.RetryTopicNamePrefix => OrganizationMemberMetadataShape.RetryTopicNamePrefix;
    int IKafkaEvent.RetryTopicCount => OrganizationMemberMetadataShape.RetryTopicCount;
    string IKafkaEvent.DeadLetterTopicName => OrganizationMemberMetadataShape.DeadLetterTopicName;
    string? IKafkaEvent.CorrelationId => null;
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Event : IKafkaEvent
{
    string IKafkaEvent.TopicName => OrganizationMemberMetadataShape.TopicName;
    string IKafkaEvent.RetryTopicNamePrefix => OrganizationMemberMetadataShape.RetryTopicNamePrefix;
    int IKafkaEvent.RetryTopicCount => OrganizationMemberMetadataShape.RetryTopicCount;
    string IKafkaEvent.DeadLetterTopicName => OrganizationMemberMetadataShape.DeadLetterTopicName;
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
