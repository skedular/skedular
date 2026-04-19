using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Testing.Shared.Database.TestSupport;

namespace Enterprise.Shared.UnitTests.Database.PostgreSql.RepositoryFactoryBaseTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RepositoryFactoryBaseShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Throw_when_db_context_has_not_been_assigned(PostgresTestRepositoryFactory sut) =>
        Should.Throw<ArgumentNullException>(() => _ = sut.DbContext);

    [Theory]
    [AutoFakeItEasyData]
    public async Task Dispose_and_clear_db_context(PostgresTestRepositoryFactory sut)
    {
        var context = new PostgresTestDbContext(
            new DbContextOptionsBuilder<PostgresTestDbContext>().UseNpgsql("Host=localhost;Database=test;Username=test;Password=test").Options,
            new CustomDbContextOptions<PostgresTestDbContext>());

        sut.SetDbContext(context);
        sut.UnitOfWork.ShouldBeSameAs(context);

        await sut.DisposeAsync();

        Should.Throw<ArgumentNullException>(() => _ = sut.DbContext);
    }
}
