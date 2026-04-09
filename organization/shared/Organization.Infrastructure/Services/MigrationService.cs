using Api.Shared.Clients.Events.Skedular.OrganizationMember.V1;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Organization.Shared.Repositories;
using OrganizationEvent = Api.Shared.Clients.Events.Skedular.Organization.V1.Event;
using OrganizationKey = Api.Shared.Clients.Events.Skedular.Organization.V1.Key;
using OrganizationInternalEvent = Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Event;
using OrganizationInternalKey = Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Key;

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
        await kafkaHelper.CreateTopicForEventAsync<OrganizationEvent>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<OrganizationKey>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<OrganizationEvent>();

        await kafkaHelper.CreateTopicForEventAsync<OrganizationInternalEvent>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<OrganizationInternalKey>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<OrganizationInternalEvent>();

        await kafkaHelper.CreateTopicForEventAsync<Event>();
        await kafkaHelper.RegisterKeyProtobufSchemaAsync<Key>();
        await kafkaHelper.RegisterValueProtobufSchemaAsync<Event>();
    }
}
