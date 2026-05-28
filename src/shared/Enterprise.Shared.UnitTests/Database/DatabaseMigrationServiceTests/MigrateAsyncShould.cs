using Enterprise.Shared.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Testing.Shared.Database.TestSupport;

namespace Enterprise.Shared.UnitTests.Database.DatabaseMigrationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MigrateAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Create_database_when_it_does_not_exist(
        DatabaseMigrationService sut,
        string dbFileName,
        CancellationToken cancellationToken)
    {
        var path = Path.Combine(Path.GetTempPath(), $"{dbFileName}.db");

        try
        {
            await using var context =
                new DatabaseTestContext(new DbContextOptionsBuilder<DatabaseTestContext>().UseSqlite($"Data Source={path}").Options);

            await sut.MigrateAsync(context, cancellationToken);

            Path.Exists(path).ShouldBeTrue();
            (await context.Database.CanConnectAsync(cancellationToken)).ShouldBeTrue();

            await context.Database.CloseConnectionAsync();
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            if (Path.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
