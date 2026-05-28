using Enterprise.Shared.Database;
using MsTeams.Shared.Repositories;

namespace MsTeams.Infrastructure.Services;

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
