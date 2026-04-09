using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.Outbox.Temporal;

public interface ITemporalSignalOutboxStore
{
    DbSet<TemporalSignalOutbox> TemporalSignalOutbox { get; set; }
}
