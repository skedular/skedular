using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.Outbox.Database;

public interface IOutboxStore
{
    DbSet<Entities.Outbox> Outbox { get; set; }
}
