using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Testing.Shared.Database.TestSupport;

namespace Enterprise.Shared.UnitTests.Database.PostgreSql.DbContextBaseTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DbContextBaseShould
{
    private static PostgresTestDbContext BuildContext(bool isPostgis = false, bool isPooled = false)
    {
        var options = new DbContextOptionsBuilder<PostgresTestDbContext>()
            .UseNpgsql("Host=localhost;Database=test;Username=test;Password=test")
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

    [Fact]
    public void Map_entity_framework_version_to_postgres_xmin()
    {
        using var ctx = BuildContext();

        var entityType = ctx.Model.FindEntityType(typeof(ParentEntity));
        var versionProperty = entityType?.FindProperty(nameof(EntityBase.EntityFrameworkVersion));

        versionProperty.ShouldNotBeNull();
        versionProperty.IsConcurrencyToken.ShouldBeTrue();
        versionProperty.ValueGenerated.ShouldBe(ValueGenerated.OnAddOrUpdate);
        versionProperty.GetColumnType().ShouldBe("xid");
        versionProperty.GetColumnName().ShouldBe("xmin");
    }
}
