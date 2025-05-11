using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Enterprise.Shared.Database;

public interface IDatabaseMigrationService
{
    Task MigrateAsync(DbContext dbContext, CancellationToken cancellationToken);
}

public class DatabaseMigrationService : IDatabaseMigrationService
{
    public async Task MigrateAsync(DbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(
            dbContext,
            async (context, ct) =>
            {
                var dbCreator = context.GetService<IRelationalDatabaseCreator>();
                if (!await dbCreator.ExistsAsync(ct))
                {
                    await dbCreator.CreateAsync(ct);
                }
            }, cancellationToken);
        await strategy.ExecuteAsync(
            dbContext,
            async (context, ct) =>
            {
                await context.Database.MigrateAsync(ct);
            }, cancellationToken);
    }
}
