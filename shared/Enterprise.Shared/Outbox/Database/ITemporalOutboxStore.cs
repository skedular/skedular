using Enterprise.Shared.Outbox.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.Outbox.Database;

public interface ITemporalOutboxStore
{
    DbSet<TemporalOutbox> TemporalOutbox { get; set; }
}
