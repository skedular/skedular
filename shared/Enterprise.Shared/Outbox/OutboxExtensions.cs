using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Publishers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Outbox;

public static class OutboxExtensions
{
    public static DatabaseSetupContext<TDbContext> AddOutboxService<TDbContext>(
        this DatabaseSetupContext<TDbContext> databaseSetup)
        where TDbContext : DbContext, IOutboxStore
    {
        databaseSetup.ServiceCollection
            .AddSingleton(typeof(IOutboxEventPublisher<,>), typeof(OutboxEventPublisher<,>))
            .AddHostedService<OutboxBackgroundService<TDbContext>>();

        databaseSetup.ServiceCollection.Decorate<IDbTransactionBuilder, OutboxTransactionBuilderDecorator>();

        return databaseSetup;
    }
}
