using Enterprise.Shared.Database;

namespace Enterprise.Shared.UnitTests.Database.ReplicatedEntityBaseTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ReplicatedEntityBaseShould
{
    [Fact]
    public void Have_event_raised_at_property()
    {
        var entity = new ReplicatedSpecEntity
        {
            Id = "1",
            CreatedAt = TimeProvider.System.GetUtcNow(),
            EventRaisedAt = TimeProvider.System.GetUtcNow(),
        };

        entity.EventRaisedAt.ShouldNotBeNull();
    }

    [Fact]
    public void Allow_null_event_raised_at()
    {
        var entity = new ReplicatedSpecEntity
        {
            Id = "1",
            CreatedAt = TimeProvider.System.GetUtcNow(),
        };

        entity.EventRaisedAt.ShouldBeNull();
    }
}

file sealed class ReplicatedSpecEntity : ReplicatedEntityBase
{
    public string? Name { get; set; }
}
