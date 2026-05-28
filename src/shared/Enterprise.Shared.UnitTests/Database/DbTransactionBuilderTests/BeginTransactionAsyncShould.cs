using Enterprise.Shared.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Testing.Shared.Database.TestSupport;

namespace Enterprise.Shared.UnitTests.Database.DbTransactionBuilderTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class BeginTransactionAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Begin_read_committed_transaction_for_db_context_unit_of_work(DbTransactionBuilder sut, CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(cancellationToken);

        await using var context = new DatabaseTestContext(new DbContextOptionsBuilder<DatabaseTestContext>().UseSqlite(connection).Options);
        await context.Database.EnsureCreatedAsync(cancellationToken);

        await using var transaction = await sut.BeginTransactionAsync(context, cancellationToken);

        transaction.ShouldNotBeNull();
        context.Database.CurrentTransaction.ShouldNotBeNull();
    }
}
