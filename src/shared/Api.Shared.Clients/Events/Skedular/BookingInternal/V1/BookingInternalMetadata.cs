using Api.Shared.Events.Kafka;

namespace Api.Shared.Clients.Events.Skedular.BookingInternal.V1;

file static class BookingInternalMetadataShape
{
    internal const string TopicName = "booking.v1.internal";
    internal const string RetryTopicNamePrefix = "booking.v1.internal.retry";
    internal const int RetryTopicCount = 1;
    internal const string DeadLetterTopicName = "booking.v1.internal.deadletter";
}

public partial class Key : IKafkaEvent
{
    string IKafkaEvent.TopicName => BookingInternalMetadataShape.TopicName;
    string IKafkaEvent.RetryTopicNamePrefix => BookingInternalMetadataShape.RetryTopicNamePrefix;
    int IKafkaEvent.RetryTopicCount => BookingInternalMetadataShape.RetryTopicCount;
    string IKafkaEvent.DeadLetterTopicName => BookingInternalMetadataShape.DeadLetterTopicName;
    string? IKafkaEvent.CorrelationId => null;
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Event : IKafkaEvent
{
    string IKafkaEvent.TopicName => BookingInternalMetadataShape.TopicName;
    string IKafkaEvent.RetryTopicNamePrefix => BookingInternalMetadataShape.RetryTopicNamePrefix;
    int IKafkaEvent.RetryTopicCount => BookingInternalMetadataShape.RetryTopicCount;
    string IKafkaEvent.DeadLetterTopicName => BookingInternalMetadataShape.DeadLetterTopicName;
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
