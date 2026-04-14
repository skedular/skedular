using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Outbox;

/// <summary>
///     Direct instance accessor for singleton context registrations.
///     Reuses the same context across calls and does not dispose of it.
/// </summary>
public class GetContextAccessor<TDbContext>(
    TDbContext dbContext,
    ILogger<GetContextAccessor<TDbContext>> logger)
    : IOutboxDbContextAccessor<TDbContext> where TDbContext : DbContext
{
    public Task<TDbContext> GetContextAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Returning shared outbox DbContext instance. DbContextType={DbContextType}", typeof(TDbContext).Name);
        return Task.FromResult(dbContext);
    }

    public Task ReleaseContextAsync(TDbContext context, CancellationToken cancellationToken)
    {
        // Direct instance is shared and managed by DI container, so we don't dispose.
        // Optionally, clear change tracking here if needed for the next operation.
        logger.LogDebug("Clearing tracked entities on shared outbox DbContext. DbContextType={DbContextType}", typeof(TDbContext).Name);
        context.ChangeTracker.Clear();
        return Task.CompletedTask;
    }
}
