using Enterprise.Shared.Database;
using Enterprise.Shared.UnitTests.Database.TestSupport;

namespace Enterprise.Shared.UnitTests.Database.SpecificationTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SpecificationShould
{
    [Fact]
    public void Store_all_mutations_on_specification()
    {
        var spec = new Specification<SpecEntity> { Criteria = item => item.Name == "expected" };

        spec.AddInclude(item => item.Parent!)
            .AddInclude(nameof(SpecEntity.Owner))
            .ApplyOrderBy(item => item.Name!)
            .ApplyOrderByDescending(item => item.CreatedAt)
            .ApplyGroupBy(item => item.ParentId!)
            .ApplyPaging(2, 5);

        spec.Criteria.ShouldNotBeNull();
        spec.Includes.Count.ShouldBe(1);
        spec.IncludeStrings.ShouldBe([nameof(SpecEntity.Owner)]);
        spec.OrderBy.ShouldNotBeNull();
        spec.OrderByDescending.ShouldNotBeNull();
        spec.GroupBy.ShouldNotBeNull();
        spec.Skip.ShouldBe(2);
        spec.Take.ShouldBe(5);
        spec.IsPagingEnabled.ShouldBeTrue();
    }
}
