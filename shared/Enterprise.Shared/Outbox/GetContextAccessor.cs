using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.Outbox;

/// <summary>
///     Direct instance accessor for singleton context registrations.
///     Reuses the same context across calls and does not dispose of it.
/// </summary>
public class GetContextAccessor<TDbContext>(TDbContext dbContext)
    : IOutboxDbContextAccessor<TDbContext> where TDbContext : DbContext
{
    public Task<TDbContext> GetContextAsync(CancellationToken cancellationToken) => Task.FromResult(dbContext);

    public Task ReleaseContextAsync(TDbContext context, CancellationToken cancellationToken)
    {
        // Direct instance is shared and managed by DI container, so we don't dispose.
        // Optionally, clear change tracking here if needed for the next operation.
        context.ChangeTracker.Clear();
        return Task.CompletedTask;
    }
}
