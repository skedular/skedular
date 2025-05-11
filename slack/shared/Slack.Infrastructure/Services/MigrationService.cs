using Api.Shared.Clients.Events.Skedular.SlackInternal.V1.Value;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Slack.Shared.Repositories;

namespace Slack.Infrastructure.Services;

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
