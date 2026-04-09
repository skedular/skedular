using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.Outbox.Kafka;

public interface IKafkaOutboxStore
{
    DbSet<KafkaOutbox> KafkaOutbox { get; set; }
}
