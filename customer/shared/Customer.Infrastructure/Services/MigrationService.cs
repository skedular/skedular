using Api.Shared.Clients.Events.Skedular.Customer.V1;
using Customer.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using CustomerReadiness_V1_Value_Event = Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1.Event;

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

        await kafkaHelper.CreateTopicForEventAsync<CustomerReadiness_V1_Value_Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<CustomerReadiness_V1_Value_Event>();
    }
}
