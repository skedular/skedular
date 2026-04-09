using Enterprise.Shared.Events;

namespace Api.Shared.Clients.Events.Skedular.BookingInternal.V1;

file static class BookingInternalMetadataShape
{
    internal const string TopicName = "booking.v1.internal";
    internal const string RetryTopicNamePrefix = "booking.v1.internal.retry";
    internal const int RetryTopicCount = 1;
    internal const string DeadLetterTopicName = "booking.v1.internal.deadletter";
}

public partial class Key : IEvent
{
    string IEvent.TopicName => BookingInternalMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => BookingInternalMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => BookingInternalMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => BookingInternalMetadataShape.DeadLetterTopicName;
}

[KafkaTopic(3, 1, 3, 3)]
public partial class Event : IEvent
{
    string IEvent.TopicName => BookingInternalMetadataShape.TopicName;
    string IEvent.RetryTopicNamePrefix => BookingInternalMetadataShape.RetryTopicNamePrefix;
    int IEvent.RetryTopicCount => BookingInternalMetadataShape.RetryTopicCount;
    string IEvent.DeadLetterTopicName => BookingInternalMetadataShape.DeadLetterTopicName;
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
