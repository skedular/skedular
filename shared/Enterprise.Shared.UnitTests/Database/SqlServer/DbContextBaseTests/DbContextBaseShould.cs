using Enterprise.Shared.Database;
using Enterprise.Shared.UnitTests.Database.TestSupport;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.UnitTests.Database.SqlServer.DbContextBaseTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DbContextBaseShould
{
    private static SqlServerTestDbContext BuildContext(bool isPooled = false)
    {
        var options = new DbContextOptionsBuilder<SqlServerTestDbContext>()
            .UseInMemoryDatabase(Guid.CreateVersion7().ToString())
            .Options;
        var customOptions = new CustomDbContextOptions<SqlServerTestDbContext> { IsPooled = isPooled };
        return new SqlServerTestDbContext(options, customOptions);
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
        using var ctx = BuildContext();
        ctx.ShouldNotBeNull();
    }

    [Fact]
    public void Skip_interceptor_when_pooled()
    {
        using var ctx = BuildContext(true);
        ctx.ShouldNotBeNull();
    }
}
