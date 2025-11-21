using System.Reflection;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Publishers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Outbox;

public static class Extensions
{
    extension(Type type)
    {
        public string ToWorkflowType() => type.FullName!;
    }

    extension(MethodInfo methodInfo)
    {
        public string ToWorkflowSignalType() => $"{methodInfo.DeclaringType}.{methodInfo.Name}";
    }

    extension(IServiceCollection services)
    {
        public IServiceCollection AddKafkaOutboxBackgroundService<TDbContext>()
            where TDbContext : DbContext, IKafkaOutboxStore =>
            services.AddHostedService<KafkaOutboxBackgroundService<TDbContext>>();

        public IServiceCollection AddKafkaOutboxService() =>
            services
                .AddSingleton(typeof(IKafkaOutboxEventPublisher<,>), typeof(KafkaOutboxEventPublisher<,>));

        public IServiceCollection AddTemporalOutboxBackgroundService<TDbContext>()
            where TDbContext : DbContext, ITemporalOutboxStore, ITemporalSignalOutboxStore =>
            services
                .AddHostedService<TemporalOutboxBackgroundService<TDbContext>>()
                .AddHostedService<TemporalSignalOutboxBackgroundService<TDbContext>>();

        public IServiceCollection AddTemporalOutboxService() =>
            services
                .AddSingleton(typeof(ITemporalOutboxWorkflowExecutor<>), typeof(TemporalOutboxWorkflowExecutor<>))
                .AddSingleton(typeof(ITemporalSignalOutboxWorkflowExecutor), typeof(TemporalSignalOutboxWorkflowExecutor));
    }
}
