using Api.Shared.Events;

namespace Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1;

file static class OrganizationInternalMetadataShape
{
    internal const string TopicName = "organization.v1.internal";
    internal const string RetryTopicNamePrefix = "organization.v1.internal.retry";
    internal const int RetryTopicCount = 1;
    internal const string DeadLetterTopicName = "organization.v1.internal.deadletter";
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Key : IEvent
{
    string IEvent.TopicName => OrganizationInternalMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => OrganizationInternalMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => OrganizationInternalMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => OrganizationInternalMetadataShape.DeadLetterTopicName;
    string? IEvent.CorrelationId => null;
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Event : IEvent
{
    string IEvent.TopicName => OrganizationInternalMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => OrganizationInternalMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => OrganizationInternalMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => OrganizationInternalMetadataShape.DeadLetterTopicName;
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
