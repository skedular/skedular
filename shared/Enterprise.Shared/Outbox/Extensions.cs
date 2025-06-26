using System.Reflection;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Publishers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Outbox;

public static class Extensions
{
    public static IServiceCollection AddKafkaOutboxBackgroundService<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext, IKafkaOutboxStore =>
        services.AddHostedService<KafkaOutboxBackgroundService<TDbContext>>();

    public static IServiceCollection AddKafkaOutboxService(this IServiceCollection services) =>
        services
            .AddSingleton(typeof(IKafkaOutboxEventPublisher<,>), typeof(KafkaOutboxEventPublisher<,>));

    public static IServiceCollection AddTemporalOutboxBackgroundService<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext, ITemporalOutboxStore, ITemporalSignalOutboxStore =>
        services
            .AddHostedService<TemporalOutboxBackgroundService<TDbContext>>()
            .AddHostedService<TemporalSignalOutboxBackgroundService<TDbContext>>();

    public static IServiceCollection AddTemporalOutboxService(this IServiceCollection services) =>
        services
            .AddSingleton(typeof(ITemporalOutboxWorkflowExecutor<>), typeof(TemporalOutboxWorkflowExecutor<>))
            .AddSingleton(typeof(ITemporalSignalOutboxWorkflowExecutor), typeof(TemporalSignalOutboxWorkflowExecutor));

    public static string ToWorkflowType(this Type type) => type.FullName!;

    public static string ToWorkflowSignalType(this MethodInfo methodInfo) => $"{methodInfo.DeclaringType}.{methodInfo.Name}";
}
