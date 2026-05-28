using Enterprise.Shared.Models;

namespace Enterprise.Shared.UnitTests.Models.ReplicatedModelTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ReplicatedModelBaseShould
{
    [Fact]
    public void Allow_event_raised_at_to_be_set()
    {
        var model = new ReplicatedModelBase
        {
            Id = "1", CreatedAt = TimeProvider.System.GetUtcNow(), EventRaisedAt = TimeProvider.System.GetUtcNow()
        };

        model.EventRaisedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Allow_event_raised_at_to_be_null()
    {
        var model = new ReplicatedModelBase { Id = "1", CreatedAt = TimeProvider.System.GetUtcNow() };

        model.EventRaisedAt.ShouldBeNull();
    }

    [Fact]
    public void Allow_replicated_model_with_deleted_to_be_set()
    {
        var model = new ReplicatedModelBaseWithDeleted
        {
            Id = "1", CreatedAt = TimeProvider.System.GetUtcNow(), DeletedAt = TimeProvider.System.GetUtcNow()
        };

        model.DeletedAt.ShouldNotBeNull();
    }
}
