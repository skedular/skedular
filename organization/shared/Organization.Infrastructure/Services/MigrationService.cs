using Api.Shared.Clients.Events.Skedular.Organization.V1.Key;
using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Organization.Shared.Repositories;

namespace Organization.Infrastructure.Services;

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

        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Value.Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Key.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Value.Event>();

        await kafkaHelper.CreateTopicForEventAsync<Api.Shared.Clients.Events.Skedular.OrganizationMember.V1.Value.Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.OrganizationMember.V1.Key.Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Api.Shared.Clients.Events.Skedular.OrganizationMember.V1.Value.Event>();
    }
}
