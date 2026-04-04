using Api.Shared.Clients.Events.Skedular.Booking.V1.Key;
using Api.Shared.Clients.Events.Skedular.Booking.V1.Value;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;

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

        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Key.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Event>();
    }
}
