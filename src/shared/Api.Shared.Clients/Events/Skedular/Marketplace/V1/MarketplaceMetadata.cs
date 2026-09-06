using Api.Shared.Events.Kafka;

namespace Api.Shared.Clients.Events.Skedular.Marketplace.V1;

file static class MarketplaceMetadataShape
{
    internal const string TopicName = "marketplace.v1.event";
    internal const string RetryTopicNamePrefix = "marketplace.v1.event.retry";
    internal const int RetryTopicCount = 1;
    internal const string DeadLetterTopicName = "marketplace.v1.event.deadletter";
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Key : IKafkaEvent
{
    string IKafkaEvent.TopicName => MarketplaceMetadataShape.TopicName;
    string IKafkaEvent.RetryTopicNamePrefix => MarketplaceMetadataShape.RetryTopicNamePrefix;
    int IKafkaEvent.RetryTopicCount => MarketplaceMetadataShape.RetryTopicCount;
    string IKafkaEvent.DeadLetterTopicName => MarketplaceMetadataShape.DeadLetterTopicName;
    string? IKafkaEvent.CorrelationId => null;
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Event : IKafkaEvent
{
    string IKafkaEvent.TopicName => MarketplaceMetadataShape.TopicName;
    string IKafkaEvent.RetryTopicNamePrefix => MarketplaceMetadataShape.RetryTopicNamePrefix;
    int IKafkaEvent.RetryTopicCount => MarketplaceMetadataShape.RetryTopicCount;
    string IKafkaEvent.DeadLetterTopicName => MarketplaceMetadataShape.DeadLetterTopicName;
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
