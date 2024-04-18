namespace Enterprise.Shared.Telemetry;

/// <summary>
///     Semantic Conventions
///     https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/messaging.md
/// </summary>
public static class SemanticConventions
{
    public const string NetPeerName = "net.peer.name";
    public const string NetHostIp = "net.host.ip";

    /// <summary>
    ///     Consumer only.
    /// </summary>
    /// <remarks>
    ///     For Apache Kafka producers, peer.service SHOULD be set to the name of the broker or service
    ///     the message will be sent to. The service.name of a Consumer's Resource SHOULD match the peer.service
    ///     of the Producer, when the message is directly passed to another service.
    ///     If an intermediary broker is present, service.name and peer.service will not be the same.
    /// </remarks>
    public const string ServiceName = "service.name";

    public const string MessagingSystem = "messaging.system";

    /// <summary>
    ///     The message destination name. This might be equal to the span name but is required nevertheless.	MyQueue; MyTopic
    /// </summary>
    public const string MessagingDestination = "messaging.destination";

    /// <summary>
    ///     The kind of message destination:	queue/topic
    /// </summary>
    public const string MessagingDestinationKind = "messaging.destination_kind";

    /// <summary>
    ///     A boolean that is true if the message destination is temporary.
    /// </summary>
    public const string MessagingTempDestination = "messaging.temp_destination";

    /// <summary>
    ///     Connection string.
    /// </summary>
    public const string MessagingUrl = "messaging.url";

    public const string MessagingMessageId = "messaging.message_id";

    /// <summary>
    ///     a.k.a Correlation ID. The conversation ID identifying the conversation to which the message belongs, represented as
    ///     a string.
    /// </summary>
    public const string MessagingConversationId = "messaging.conversation_id";

    public const string MessagingPayloadSize = "messaging.message_payload_size_bytes";

    public const string MessagingPayloadCompressedSize = "messaging.message_payload_compressed_size_bytes";

    /// <summary>
    ///     A string identifying the kind of message consumption as defined in the Operation names section above. If the
    ///     operation is "send", this attribute MUST NOT be set, since the operation can be inferred from the span kind in that
    ///     case.
    /// </summary>
    /// <remarks>
    ///     Consumers Only.
    /// </remarks>
    /// <example> `receive` or `process` only</example>
    public const string MessagingOperation = "messaging.operation";

    public const string MessagingKafkaMessageKey = "messaging.kafka.message_key";
    public const string MessagingKafkaConsumerGroup = "messaging.kafka.consumer_group";
    public const string MessagingKafkaClientId = "messaging.kafka.client_id";

    /// <summary>
    ///     int - Partition the message is sent to.
    /// </summary>
    public const string MessagingKafkaPartition = "messaging.kafka.partition";

    public const string MessagingKafkaTombstone = "messaging.kafka.tombstone";

    /// <summary>
    ///     Producers only
    /// </summary>
    /// <remarks>
    ///     For Apache Kafka producers, peer.service SHOULD be set to the name of the broker or service
    ///     the message will be sent to. The service.name of a Consumer's Resource SHOULD match the peer.service
    ///     of the Producer, when the message is directly passed to another service.
    ///     If an intermediary broker is present, service.name and peer.service will not be the same.
    /// </remarks>
    public const string PeerService = "peer.service";
}
