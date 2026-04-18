using Api.Shared.Events;

namespace Api.Shared.Clients.Events.Skedular.Marketplace.V1;

file static class MarketplaceMetadataShape
{
    internal const string TopicName = "marketplace.v1.event";
    internal const string RetryTopicNamePrefix = "marketplace.v1.event.retry";
    internal const int RetryTopicCount = 1;
    internal const string DeadLetterTopicName = "marketplace.v1.event.deadletter";
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Key : IEvent
{
    string IEvent.TopicName => MarketplaceMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => MarketplaceMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => MarketplaceMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => MarketplaceMetadataShape.DeadLetterTopicName;
    string? IEvent.CorrelationId => null;
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Event : IEvent
{
    string IEvent.TopicName => MarketplaceMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => MarketplaceMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => MarketplaceMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => MarketplaceMetadataShape.DeadLetterTopicName;
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
