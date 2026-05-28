using Api.Shared.Clients.Events.Skedular.Booking.V1;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Event = Api.Shared.Clients.Events.Skedular.Booking.V1.Event;
using Value_Event = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Event;

namespace Booking.Infrastructure.Services;

public interface IMigrationService
{
    Task MigrateAsync(CancellationToken cancellationToken);
}

public class MigrationService(IKafkaHelper kafkaHelper, IRepositoryFactory repositoryFactory, IDatabaseMigrationService databaseMigrationService)
    : IMigrationService
{
    public async Task MigrateAsync(CancellationToken cancellationToken) =>
        await Task.WhenAll(databaseMigrationService.MigrateAsync(repositoryFactory.DbContext, cancellationToken), CreateTopicsAsync());

    private async Task CreateTopicsAsync()
    {
        await kafkaHelper.CreateTopicForEventAsync<Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Event>();

        await kafkaHelper.CreateTopicForEventAsync<Value_Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Value_Event>();
    }
}
