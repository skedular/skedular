using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Database;

public interface IDatabaseMigrationService
{
    Task MigrateAsync(DbContext dbContext, CancellationToken cancellationToken);
}

public class DatabaseMigrationService(ILogger<DatabaseMigrationService> logger) : IDatabaseMigrationService
{
    public async Task MigrateAsync(DbContext dbContext, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting database migration for context {DbContextType}", dbContext.GetType().Name);

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(
            dbContext,
            async (context, ct) =>
            {
                var dbCreator = context.GetService<IRelationalDatabaseCreator>();
                if (!await dbCreator.ExistsAsync(ct))
                {
                    logger.LogInformation("Database does not exist for context {DbContextType}; creating it", context.GetType().Name);
                    await dbCreator.CreateAsync(ct);
                }
                else
                {
                    logger.LogDebug("Database already exists for context {DbContextType}", context.GetType().Name);
                }
            },
            cancellationToken);

        logger.LogInformation("Applying pending migrations for context {DbContextType}", dbContext.GetType().Name);
        await strategy.ExecuteAsync(dbContext, async (context, ct) => { await context.Database.MigrateAsync(ct); }, cancellationToken);
        logger.LogInformation("Completed database migration for context {DbContextType}", dbContext.GetType().Name);
    }
}
