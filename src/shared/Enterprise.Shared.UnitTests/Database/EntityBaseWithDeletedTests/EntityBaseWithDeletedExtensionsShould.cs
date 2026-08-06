using Enterprise.Shared.Database;
using Testing.Shared.Database.TestSupport;

namespace Enterprise.Shared.UnitTests.Database.EntityBaseWithDeletedTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EntityBaseWithDeletedExtensionsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Report_deleted_state_for_entity_base_with_deleted(DateTimeOffset deleteAt)
    {
        var activeEntity = new SpecEntity();
        var deletedEntity = new SpecEntity
        {
            DeletedAt = deleteAt,
        };

        activeEntity.IsDeleted().ShouldBeFalse();
        activeEntity.IsNotDeleted().ShouldBeTrue();
        deletedEntity.IsDeleted().ShouldBeTrue();
        deletedEntity.IsNotDeleted().ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Report_deleted_state_for_replicated_entity_base_with_deleted(DateTimeOffset deleteAt)
    {
        var activeEntity = new ReplicatedDeletedEntity();
        var deletedEntity = new ReplicatedDeletedEntity
        {
            DeletedAt = deleteAt,
        };

        activeEntity.IsReplicatedDeleted().ShouldBeFalse();
        activeEntity.IsReplicatedNotDeleted().ShouldBeTrue();
        deletedEntity.IsReplicatedDeleted().ShouldBeTrue();
        deletedEntity.IsReplicatedNotDeleted().ShouldBeFalse();
    }
}
