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
}
