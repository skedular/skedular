using Api.Shared.Clients.Events.Skedular.MsTeamsInternal.V1.Value;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using MsTeams.Shared.Repositories;

namespace MsTeams.Infrastructure.Services;

public interface IMigrationService
{
    Task MigrateAsync(CancellationToken cancellationToken);
}

public class MigrationService(IKafkaHelper kafkaHelper, IRepositoryFactory repositoryFactory, IDatabaseMigrationService databaseMigrationService)
    : IMigrationService
{
    public async Task MigrateAsync(CancellationToken cancellationToken) =>
        await Task.WhenAll(databaseMigrationService.MigrateAsync(repositoryFactory.DbContext, cancellationToken), CreateTopicsAsync());

    private async Task CreateTopicsAsync() => await kafkaHelper.CreateTopicForEventAsync<Event>();
    // await kafkaHelper.RegisterKeyProtobufSchemaAsync<Key>();
    // await kafkaHelper.RegisterValueProtobufSchemaAsync<Event>();
}
