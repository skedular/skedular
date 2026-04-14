using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Outbox;

/// <summary>
///     Factory-based accessor for pooled or non-pooled factory registrations.
///     Creates a new context per access and disposes after use.
/// </summary>
public class FactoryBasedOutboxDbContextAccessor<TDbContext>(
    IDbContextFactory<TDbContext> contextFactory,
    ILogger<FactoryBasedOutboxDbContextAccessor<TDbContext>> logger)
    : IOutboxDbContextAccessor<TDbContext> where TDbContext : DbContext
{
    public Task<TDbContext> GetContextAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug("Creating outbox DbContext from factory. DbContextType={DbContextType}", typeof(TDbContext).Name);
        return contextFactory.CreateDbContextAsync(cancellationToken);
    }

    public async Task ReleaseContextAsync(TDbContext context, CancellationToken cancellationToken)
    {
        logger.LogDebug("Disposing factory-created outbox DbContext. DbContextType={DbContextType}", typeof(TDbContext).Name);
        await context.DisposeAsync();
    }
}
