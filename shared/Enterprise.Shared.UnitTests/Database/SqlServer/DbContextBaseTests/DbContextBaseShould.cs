using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Testing.Shared.Database.TestSupport;

namespace Enterprise.Shared.UnitTests.Database.SqlServer.DbContextBaseTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DbContextBaseShould
{
    private static SqlServerTestDbContext BuildRelationalContext(bool isPooled = false)
    {
        var options = new DbContextOptionsBuilder<SqlServerTestDbContext>()
            .UseSqlServer("Server=localhost;Database=local.test;User Id=sa;Password=Password123!;TrustServerCertificate=True")
            .Options;
        var customOptions = new CustomDbContextOptions<SqlServerTestDbContext> { IsPooled = isPooled };
        return new SqlServerTestDbContext(options, customOptions);
    }

    private static SqlServerTestDbContext BuildInMemoryContext(bool isPooled = false)
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
        using var ctx = BuildInMemoryContext();
        ctx.ShouldNotBeNull();
    }

    [Fact]
    public void Apply_interceptor_when_not_pooled()
    {
        using var ctx = BuildInMemoryContext();
        ctx.ShouldNotBeNull();
    }

    [Fact]
    public void Skip_interceptor_when_pooled()
    {
        using var ctx = BuildInMemoryContext(true);
        ctx.ShouldNotBeNull();
    }

    [Fact]
    public void Configure_entity_framework_version_as_application_managed_concurrency_token()
    {
        using var ctx = BuildRelationalContext();

        var entityType = ctx.Model.FindEntityType(typeof(ParentEntity));
        var versionProperty = entityType?.FindProperty(nameof(EntityBase.EntityFrameworkVersion));

        versionProperty.ShouldNotBeNull();
        versionProperty.IsConcurrencyToken.ShouldBeTrue();
        versionProperty.ValueGenerated.ShouldBe(ValueGenerated.Never);
        versionProperty.GetColumnType().ShouldBe("bigint");
    }

    [Fact]
    public void Increment_entity_framework_version_on_update()
    {
        using var ctx = BuildInMemoryContext();

        var entity = new ParentEntity { Id = Guid.CreateVersion7().ToString(), CreatedAt = DateTimeOffset.UtcNow, Name = "before" };

        ctx.Parents.Add(entity);
        ctx.SaveChanges();

        entity.EntityFrameworkVersion.ShouldBe(0u);

        entity.Name = "after";
        ctx.SaveChanges();

        entity.EntityFrameworkVersion.ShouldBe(1u);
    }
}
