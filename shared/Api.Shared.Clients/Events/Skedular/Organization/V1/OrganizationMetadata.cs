using Enterprise.Shared.Events;

namespace Api.Shared.Clients.Events.Skedular.Organization.V1;

file static class OrganizationMetadataShape
{
    internal const string TopicName = "organization.v1.event";
    internal const string RetryTopicNamePrefix = "organization.v1.event.retry";
    internal const int RetryTopicCount = 1;
    internal const string DeadLetterTopicName = "organization.v1.event.deadletter";
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Key : IEvent
{
    string IEvent.TopicName => OrganizationMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => OrganizationMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => OrganizationMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => OrganizationMetadataShape.DeadLetterTopicName;
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Event : IEvent
{
    string IEvent.TopicName => OrganizationMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => OrganizationMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => OrganizationMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => OrganizationMetadataShape.DeadLetterTopicName;
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
