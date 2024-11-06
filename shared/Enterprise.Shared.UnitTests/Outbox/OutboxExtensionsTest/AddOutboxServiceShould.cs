using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Outbox.Database;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.UnitTests.Database.ServiceExtensionsTests;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Xunit;

namespace Enterprise.Shared.UnitTests.Outbox.OutboxExtensionsTest;

public class AddOutboxServiceShould
{
    [Fact]
    public void Add_Services()
    {
        var collection = new ServiceCollection();
        var dataSource = NpgsqlDataSource.Create("host=123");
        var dbSetupContext = new DatabaseSetupContext<DummyDbContext>(new DatabaseSetupStub(collection, dataSource));
        collection.AddSingleton<IDbTransactionBuilder, DbTransactionBuilder>();

        dbSetupContext.AddOutboxService();

        using (new AssertionScope())
        {
            collection.Should()
                .ContainSingle(descriptor =>
                    descriptor.ServiceType == typeof(IOutboxEventPublisher<,>) &&
                    descriptor.ImplementationType == typeof(OutboxEventPublisher<,>));
        }
    }

    public class DummyDbContext(DbContextOptions options) : DbContext(options), IOutboxStore
    {
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
        public DbSet<Shared.Outbox.Database.Entities.Outbox>? Outbox { get; }
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    }
}
