using Confluent.Kafka;

namespace Enterprise.Shared.Kafka.Consume;

public record EventContext(ConsumeResult<byte[], byte[]> ConsumeResult);
