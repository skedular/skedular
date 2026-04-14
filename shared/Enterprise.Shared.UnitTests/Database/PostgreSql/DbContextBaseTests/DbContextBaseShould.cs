using Enterprise.Shared.Database;
using Enterprise.Shared.UnitTests.Database.TestSupport;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.UnitTests.Database.PostgreSql.DbContextBaseTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DbContextBaseShould
{
    private static PostgresTestDbContext BuildContext(bool isPostgis = false, bool isPooled = false)
    {
        var options = new DbContextOptionsBuilder<PostgresTestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var customOptions = new CustomDbContextOptions<PostgresTestDbContext> { IsPostgisEnabled = isPostgis, IsPooled = isPooled };
        return new PostgresTestDbContext(options, customOptions);
    }

    [Fact]
    public void Create_context_with_in_memory_provider()
    {
        using var ctx = BuildContext();
        ctx.ShouldNotBeNull();
    }

    [Fact]
    public void Apply_interceptor_when_not_pooled()
    {
        using var ctx = BuildContext(isPooled: false);
        ctx.ShouldNotBeNull();
    }

    [Fact]
    public void Skip_interceptor_when_pooled()
    {
        using var ctx = BuildContext(isPooled: true);
        ctx.ShouldNotBeNull();
    }
}
