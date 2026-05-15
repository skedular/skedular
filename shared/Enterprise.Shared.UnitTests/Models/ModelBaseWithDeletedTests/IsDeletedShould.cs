using Enterprise.Shared.Models;

namespace Enterprise.Shared.UnitTests.Models.ModelBaseWithDeletedTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class IsDeletedShould
{
    [Fact]
    public void Return_false_when_not_deleted()
    {
        var model = new ModelBaseWithDeleted { Id = "1", CreatedAt = TimeProvider.System.GetUtcNow() };

        model.IsDeleted().ShouldBeFalse();
        model.IsNotDeleted().ShouldBeTrue();
    }

    [Fact]
    public void Return_true_when_deleted()
    {
        var model = new ModelBaseWithDeleted { Id = "1", CreatedAt = TimeProvider.System.GetUtcNow(), DeletedAt = TimeProvider.System.GetUtcNow() };

        model.IsDeleted().ShouldBeTrue();
        model.IsNotDeleted().ShouldBeFalse();
    }

    [Fact]
    public void Replicated_model_returns_false_when_not_deleted()
    {
        var model = new ReplicatedModelBaseWithDeleted { Id = "1", CreatedAt = TimeProvider.System.GetUtcNow() };

        model.IsReplicatedDeleted().ShouldBeFalse();
        model.IsReplicatedNotDeleted().ShouldBeTrue();
    }

    [Fact]
    public void Replicated_model_returns_true_when_deleted()
    {
        var model = new ReplicatedModelBaseWithDeleted
        {
            Id = "1", CreatedAt = TimeProvider.System.GetUtcNow(), DeletedAt = TimeProvider.System.GetUtcNow()
        };

        model.IsReplicatedDeleted().ShouldBeTrue();
        model.IsReplicatedNotDeleted().ShouldBeFalse();
    }
}
