using Enterprise.Shared.Database;
using Slack.Shared.Repositories;

namespace Slack.Infrastructure.Services;

public interface IMigrationService
{
    Task MigrateAsync(CancellationToken cancellationToken);
}

public class MigrationService(IRepositoryFactory repositoryFactory, IDatabaseMigrationService databaseMigrationService)
    : IMigrationService
{
    public async Task MigrateAsync(CancellationToken cancellationToken) =>
        await databaseMigrationService.MigrateAsync(repositoryFactory.DbContext, cancellationToken);
}
