using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.Outbox.Temporal;

public interface ITemporalOutboxStore
{
    DbSet<TemporalOutbox> TemporalOutbox { get; set; }
}
