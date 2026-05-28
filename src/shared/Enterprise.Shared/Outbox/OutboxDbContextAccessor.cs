using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.Outbox;

/// <summary>
///     Adapts database context access for outbox services based on the registration configuration.
///     Automatically handles both factory-based (pooled/non-pooled) and direct instance patterns.
/// </summary>
public interface IOutboxDbContextAccessor<TDbContext> where TDbContext : DbContext
{
    /// <summary>
    ///     Gets a context for use. If configured with a factory, creates a new instance.
    ///     If configured with a direct instance, returns the shared instance.
    /// </summary>
    Task<TDbContext> GetContextAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Releases a context after use. If factory-based, disposes of it.
    ///     If instance-based, this may clear tracking but does not dispose.
    /// </summary>
    Task ReleaseContextAsync(TDbContext context, CancellationToken cancellationToken);
}
