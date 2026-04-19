using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Testing.Shared.Database.TestSupport;

namespace Enterprise.Shared.UnitTests.Database.EntityBaseTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ConfigureEntityBaseShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Configure_expected_key_indexes_and_row_version_metadata(string databaseName)
    {
        using var context =
            new DatabaseTestContext(new DbContextOptionsBuilder<DatabaseTestContext>().UseInMemoryDatabase(databaseName).Options);

        var entityType = context.Model.FindEntityType(typeof(SpecEntity));

        entityType.ShouldNotBeNull();
        entityType.FindPrimaryKey()!.Properties.Single().Name.ShouldBe(nameof(EntityBase.Id));
        entityType.FindProperty(nameof(EntityBase.Id))!.GetMaxLength().ShouldBe(Constants.MaxUniqueIdLength);

        var versionProperty = entityType.FindProperty(nameof(EntityBase.EntityFrameworkVersion));
        versionProperty.ShouldNotBeNull();
        versionProperty.IsConcurrencyToken.ShouldBeTrue();

        var indexNames = entityType.GetIndexes().Select(index => index.Properties.Single().Name).ToList();

        indexNames.ShouldContain(nameof(EntityBase.CreatedAt));
        indexNames.ShouldContain(nameof(EntityBase.ModifiedAt));
        indexNames.ShouldContain(nameof(EntityBaseWithDeleted.DeletedAt));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Configure_replicated_entity_with_deleted_expected_indexes(string databaseName)
    {
        using var context =
            new DatabaseTestContext(new DbContextOptionsBuilder<DatabaseTestContext>().UseInMemoryDatabase(databaseName).Options);

        var entityType = context.Model.FindEntityType(typeof(ReplicatedDeletedEntity));

        entityType.ShouldNotBeNull();

        var indexNames = entityType.GetIndexes().Select(index => index.Properties.Single().Name).ToList();

        indexNames.ShouldContain(nameof(EntityBase.CreatedAt));
        indexNames.ShouldContain(nameof(EntityBase.ModifiedAt));
        indexNames.ShouldContain(nameof(EntityBaseWithDeleted.DeletedAt));
    }
}
