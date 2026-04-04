using Api.Shared.Clients.Events.Skedular.Customer.V1.Key;
using Api.Shared.Clients.Events.Skedular.Customer.V1.Value;
using Customer.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;

namespace Customer.Infrastructure.Services;

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
    }
}
