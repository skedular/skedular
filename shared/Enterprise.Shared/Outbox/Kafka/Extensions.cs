using Enterprise.Shared.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Outbox.Kafka;

/// <summary>
///     Modular registration extensions for the Kafka outbox pattern.
///     Use these focused registrations instead of the legacy root outbox namespace entry point.
/// </summary>
public static class Extensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers the Kafka outbox background service that drains <c>KafkaOutbox</c> rows and publishes
        ///     them to Kafka. Automatically adapts to the registered database context configuration (pooled factory,
        ///     non-pooled factory, or direct singleton instance) via <see cref="IOutboxDbContextAccessor{TDbContext}" />.
        ///     <para>
        ///         Prerequisites: <see cref="Microsoft.EntityFrameworkCore.IDbContextFactory{TDbContext}" /> or
        ///         singleton <typeparamref name="TDbContext" />, and <see cref="Enterprise.Shared.Kafka.Produce.IProducerFactory" />
        ///         must be registered.
        ///     </para>
        ///     <para>
        ///         The background service uses a polling/lease model with SKIP LOCKED to safely handle multiple
        ///         worker instances without double-publishing.
        ///     </para>
        /// </summary>
        public IServiceCollection AddKafkaOutboxBackgroundService<TDbContext>() where TDbContext : DbContext, IKafkaOutboxStore =>
            services
                .AddOutboxDbContextAccessor<TDbContext>()
                .AddHostedService<KafkaOutboxBackgroundService<TDbContext>>();

        /// <summary>
        ///     Registers the open-generic <see cref="IKafkaOutboxEventPublisher{TKey,TEvent}" /> for writing
        ///     Kafka events to the outbox table atomically within a unit of work transaction.
        ///     Already called by <c>AddKafka()</c> — only call this manually if Kafka is not used but
        ///     the Kafka outbox event publisher is still required.
        /// </summary>
        public IServiceCollection AddKafkaOutboxService()
        {
            services
                .AddSingleton(typeof(IKafkaOutboxEventPublisher<,>), typeof(KafkaOutboxEventPublisher<,>))
                .AddKafkaActivitySource(TelemetryKeys.KafkaActivitySourceName);
            return services.AddKafkaOutboxTelemetry();
        }

        public IServiceCollection AddKafkaOutboxTelemetry()
        {
            services.AddOpenTelemetry().WithTracing(builder => builder.AddSource(TelemetryKeys.KafkaActivitySourceName));
            return services;
        }
    }
}
