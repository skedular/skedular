using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.Outbox;

/// <summary>
///     Factory-based accessor for pooled or non-pooled factory registrations.
///     Creates a new context per access and disposes after use.
/// </summary>
public class FactoryBasedOutboxDbContextAccessor<TDbContext>(IDbContextFactory<TDbContext> contextFactory)
    : IOutboxDbContextAccessor<TDbContext> where TDbContext : DbContext
{
    public Task<TDbContext> GetContextAsync(CancellationToken cancellationToken) =>
        contextFactory.CreateDbContextAsync(cancellationToken);

    public async Task ReleaseContextAsync(TDbContext context, CancellationToken cancellationToken) => await context.DisposeAsync();
}
