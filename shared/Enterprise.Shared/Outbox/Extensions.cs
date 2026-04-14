using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Outbox;

/// <summary>
///     Shared extensions for outbox pattern registration.
/// </summary>
public static class Extensions
{
    /// <summary>
    ///     Registers <see cref="IOutboxDbContextAccessor{TDbContext}" /> by detecting which database context
    ///     registration pattern is in use (factory-based or direct singleton instance).
    ///     <para>
    ///         - If <see cref="IDbContextFactory{TDbContext}" /> is registered, uses <see cref="FactoryBasedOutboxDbContextAccessor{TDbContext}" />
    ///         - If <typeparamref name="TDbContext" /> is registered as a singleton, uses <see cref="GetContextAccessor{TDbContext}" />
    ///     </para>
    /// </summary>
    public static IServiceCollection AddOutboxDbContextAccessor<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        // Check if a factory is registered (pooled or non-pooled)
        if (services.Any(descriptor => descriptor.ServiceType == typeof(IDbContextFactory<TDbContext>)))
        {
            return services
                .AddSingleton<IOutboxDbContextAccessor<TDbContext>>(provider =>
                    ActivatorUtilities.CreateInstance<FactoryBasedOutboxDbContextAccessor<TDbContext>>(provider));
        }

        // Check if a direct instance is registered
        if (services.Any(descriptor => descriptor.ServiceType == typeof(TDbContext)))
        {
            return services
                .AddSingleton<IOutboxDbContextAccessor<TDbContext>>(provider =>
                    ActivatorUtilities.CreateInstance<GetContextAccessor<TDbContext>>(
                        provider,
                        provider.GetRequiredService<TDbContext>()));
        }

        throw new InvalidOperationException(
            $"Neither IDbContextFactory<{typeof(TDbContext).Name}> nor {typeof(TDbContext).Name} instance is registered. " +
            $"Register the database context first using WithPooledDbContext, WithDbContext, WithPooledDbContextFactory, or WithDbContextFactory.");
    }
}
