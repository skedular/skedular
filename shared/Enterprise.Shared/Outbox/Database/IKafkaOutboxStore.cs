using Enterprise.Shared.Outbox.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.Outbox.Database;

public interface IKafkaOutboxStore
{
    DbSet<KafkaOutbox> KafkaOutbox { get; set; }
}
