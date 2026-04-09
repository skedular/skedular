using Enterprise.Shared.Models;

namespace Enterprise.Shared.UnitTests.Models.ModelBaseTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class IsNotModifiedShould
{
    [Fact]
    public void Return_true_when_no_modified_date()
    {
        var model = new ModelBase { Id = "1", CreatedAt = DateTimeOffset.UtcNow };

        model.IsNotModified().ShouldBeTrue();
    }

    [Fact]
    public void Return_false_when_modified_date_is_set()
    {
        var model = new ModelBase { Id = "1", CreatedAt = DateTimeOffset.UtcNow, ModifiedAt = DateTimeOffset.UtcNow };

        model.IsNotModified().ShouldBeFalse();
    }
}
