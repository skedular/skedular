using Enterprise.Shared.Messaging.Kafka.Consume;
using Enterprise.Shared.Messaging.Nats.Consume;

namespace Enterprise.Shared.Messaging;

public record EventContext(KafkaEventContext? KafkaEventContext, NatsEventContext? NatsEventContext);
