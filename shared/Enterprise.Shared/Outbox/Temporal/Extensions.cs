using Enterprise.Shared.Telemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Outbox.Temporal;

/// <summary>
///     Modular registration extensions for the Temporal workflow outbox pattern.
///     Use these focused registrations instead of the legacy root outbox namespace entry point.
/// </summary>
public static class Extensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers both the <c>TemporalOutboxBackgroundService</c> (workflow starts) and
        ///     <c>TemporalSignalOutboxBackgroundService</c> (workflow signals) which drain outbox rows and
        ///     execute them via <see cref="ITemporalOutboxExecutor" /> / <see cref="ITemporalSignalOutboxExecutor" />.
        ///     Automatically adapts to the registered database context configuration (pooled factory,
        ///     non-pooled factory, or direct singleton instance) via <see cref="IOutboxDbContextAccessor{TDbContext}" />.
        ///     <para>
        ///         Prerequisites: <see cref="Microsoft.EntityFrameworkCore.IDbContextFactory{TDbContext}" /> or
        ///         singleton <typeparamref name="TDbContext" />, <c>ITemporalOutboxExecutor</c>, and
        ///         <c>ITemporalSignalOutboxExecutor</c> must be registered (typically by the domain that owns the Temporal client connection).
        ///     </para>
        /// </summary>
        public IServiceCollection AddTemporalOutboxBackgroundService<TDbContext>()
            where TDbContext : DbContext, ITemporalOutboxStore, ITemporalSignalOutboxStore =>
            services
                .AddTemporalOutboxActivitySource()
                .AddTemporalOutboxTelemetry()
                .AddOutboxDbContextAccessor<TDbContext>()
                .AddHostedService<TemporalOutboxBackgroundService<TDbContext>>()
                .AddHostedService<TemporalSignalOutboxBackgroundService<TDbContext>>();

        /// <summary>
        ///     Registers <see cref="ITemporalOutboxWorkflowExecutor" /> and
        ///     <see cref="ITemporalSignalOutboxWorkflowExecutor" /> for writing workflow start and signal
        ///     entries to the outbox table atomically within a unit of work transaction.
        ///     Already called by <c>AddTemporalWorker()</c> — only call this manually when a Temporal
        ///     worker is not hosted but the outbox writers are still needed (e.g. a pure API service).
        /// </summary>
        public IServiceCollection AddTemporalOutboxService() =>
            services
                .AddTemporalOutboxActivitySource()
                .AddTemporalOutboxTelemetry()
                .AddSingleton<ITemporalOutboxWorkflowExecutor, TemporalOutboxWorkflowExecutor>()
                .AddSingleton<ITemporalSignalOutboxWorkflowExecutor, TemporalSignalOutboxWorkflowExecutor>();

        public IServiceCollection AddTemporalOutboxTelemetry()
        {
            services.AddOpenTelemetry().WithTracing(builder => builder.AddSource(TelemetryKeys.TemporalActivitySourceName));
            return services;
        }

        private IServiceCollection AddTemporalOutboxActivitySource()
        {
            if (services.Any(item => item.ServiceType == typeof(TemporalOutboxActivitySourceRegistration)))
            {
                return services;
            }

            services.AddSingleton<IActivitySource>(_ => new ActivitySourceFacade(TelemetryKeys.TemporalActivitySourceName));
            return services.AddSingleton<TemporalOutboxActivitySourceRegistration>();
        }
    }

    private sealed class TemporalOutboxActivitySourceRegistration;
}
