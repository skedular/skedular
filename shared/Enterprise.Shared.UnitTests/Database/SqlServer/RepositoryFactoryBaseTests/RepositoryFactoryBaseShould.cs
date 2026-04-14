using Enterprise.Shared.Database;
using Enterprise.Shared.UnitTests.Database.TestSupport;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.UnitTests.Database.SqlServer.RepositoryFactoryBaseTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RepositoryFactoryBaseShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Throw_when_db_context_has_not_been_assigned(SqlServerTestRepositoryFactory sut) =>
        Should.Throw<ArgumentNullException>(() => _ = sut.DbContext);

    [Theory]
    [AutoFakeItEasyData]
    public async Task Dispose_and_clear_db_context(SqlServerTestRepositoryFactory sut)
    {
        var context = new SqlServerTestDbContext(
            new DbContextOptionsBuilder<SqlServerTestDbContext>()
                .UseSqlServer("Server=localhost;Database=test;User Id=sa;Password=Password123!;TrustServerCertificate=True")
                .Options,
            new CustomDbContextOptions<SqlServerTestDbContext>());
        sut.SetDbContext(context);

        sut.UnitOfWork.ShouldBeSameAs(context);

        await sut.DisposeAsync();

        Should.Throw<ArgumentNullException>(() => _ = sut.DbContext);
    }
}
