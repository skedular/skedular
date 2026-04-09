using Enterprise.Shared.Events;

namespace Api.Shared.Clients.Events.Skedular.Booking.V1;

file static class BookingMetadataShape
{
    internal const string TopicName = "booking.v1.event";
    internal const string RetryTopicNamePrefix = "booking.v1.event.retry";
    internal const int RetryTopicCount = 1;
    internal const string DeadLetterTopicName = "booking.v1.event.deadletter";
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Key : IEvent
{
    string IEvent.TopicName => BookingMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => BookingMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => BookingMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => BookingMetadataShape.DeadLetterTopicName;
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Event : IEvent
{
    string IEvent.TopicName => BookingMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => BookingMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => BookingMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => BookingMetadataShape.DeadLetterTopicName;
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
