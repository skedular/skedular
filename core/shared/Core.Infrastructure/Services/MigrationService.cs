using Core.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;

namespace Core.Infrastructure.Services;

public interface IMigrationService
{
    Task MigrateAsync(CancellationToken cancellationToken);
}

public class MigrationService(IKafkaHelper kafkaHelper, IRepositoryFactory repositoryFactory, IDatabaseMigrationService databaseMigrationService)
    : IMigrationService
{
    public async Task MigrateAsync(CancellationToken cancellationToken) =>
        await databaseMigrationService.MigrateAsync(repositoryFactory.DbContext, cancellationToken);
}
