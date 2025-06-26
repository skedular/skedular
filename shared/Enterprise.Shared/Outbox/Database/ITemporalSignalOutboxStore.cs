using Enterprise.Shared.Outbox.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.Outbox.Database;

public interface ITemporalSignalOutboxStore
{
    DbSet<TemporalSignalOutbox> TemporalSignalOutbox { get; set; }
}
