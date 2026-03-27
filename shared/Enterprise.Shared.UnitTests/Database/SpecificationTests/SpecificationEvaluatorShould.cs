using Enterprise.Shared.Database;
using Enterprise.Shared.UnitTests.Database.TestSupport;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.UnitTests.Database.SpecificationTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SpecificationEvaluatorShould
{
    [Fact]
    public void Return_original_query_when_specification_is_null()
    {
        using var context = CreateContext();
        Seed(context);

        var result = SpecificationEvaluator<SpecEntity>.GetQuery(context.Specs, null).ToList();

        result.Count.ShouldBe(3);
    }

    [Fact]
    public void Apply_criteria_ordering_includes_and_paging()
    {
        var databaseName = Guid.NewGuid().ToString();
        using var seedContext = CreateContext(databaseName);
        Seed(seedContext);

        using var context = CreateContext(databaseName);

        var spec = new Specification<SpecEntity> { Criteria = item => item.ParentId == "parent-1" };

        spec.AddInclude(item => item.Parent!)
            .AddInclude(nameof(SpecEntity.Owner))
            .ApplyOrderByDescending(item => item.CreatedAt)
            .ApplyPaging(1, 1);

        var result = SpecificationEvaluator<SpecEntity>.GetQuery(context.Specs.AsQueryable(), spec).Single();

        result.Id.ShouldBe("spec-1");
        result.Parent.ShouldNotBeNull();
        result.Owner.ShouldNotBeNull();
    }

    [Fact]
    public void Apply_group_by_when_specification_requests_it()
    {
        using var context = CreateContext();
        Seed(context);

        var spec = new Specification<SpecEntity>().ApplyGroupBy(item => item.ParentId!);
        var result = SpecificationEvaluator<SpecEntity>.GetQuery(context.Specs.ToList().AsQueryable(), spec).ToList();

        result.Count.ShouldBe(3);
    }

    private static DatabaseTestContext CreateContext(string? databaseName = null) =>
        new(new DbContextOptionsBuilder<DatabaseTestContext>().UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString()).Options);

    private static void Seed(DatabaseTestContext context)
    {
        if (context.Specs.Any())
        {
            return;
        }

        var parent = new ParentEntity { Id = "parent-1", CreatedAt = DateTimeOffset.UtcNow, Name = "Parent" };
        var owner = new OwnerEntity { Id = "owner-1", CreatedAt = DateTimeOffset.UtcNow, Name = "Owner" };

        context.AddRange(
            parent,
            owner,
            new SpecEntity
            {
                Id = "spec-1",
                CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-2),
                Name = "Alpha",
                Parent = parent,
                ParentId = parent.Id,
                Owner = owner,
                OwnerId = owner.Id
            },
            new SpecEntity
            {
                Id = "spec-2",
                CreatedAt = DateTimeOffset.UtcNow.AddMinutes(-1),
                Name = "Beta",
                Parent = parent,
                ParentId = parent.Id,
                Owner = owner,
                OwnerId = owner.Id
            },
            new SpecEntity { Id = "spec-3", CreatedAt = DateTimeOffset.UtcNow, Name = "Gamma", ParentId = "parent-2" });

        context.SaveChanges();
    }
}
