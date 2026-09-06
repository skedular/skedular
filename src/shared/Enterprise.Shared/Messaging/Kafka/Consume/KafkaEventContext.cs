using Confluent.Kafka;

namespace Enterprise.Shared.Messaging.Kafka.Consume;

public record KafkaEventContext(ConsumeResult<byte[], byte[]> ConsumeResult);
