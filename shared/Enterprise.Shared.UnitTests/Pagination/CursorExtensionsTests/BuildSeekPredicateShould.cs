using System.Reflection;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;

namespace Enterprise.Shared.UnitTests.Pagination.CursorExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class BuildSeekPredicateShould
{
    [Fact]
    public void Build_forward_predicate_for_uploaded_at_and_id_cursor()
    {
        var fields = new List<KeysetPaginationField<TestEntity>>
        {
            KeysetPaginationField<TestEntity>.Create(nameof(TestEntity.UploadedAt), item => item.UploadedAt, OrderDirection.Descending),
            KeysetPaginationField<TestEntity>.Create(nameof(EntityBase.Id), item => item.Id, OrderDirection.Ascending)
        };

        var payload = new KeysetCursorPayload(
            "img-1",
            new Dictionary<string, string?>
            {
                [nameof(TestEntity.UploadedAt)] = "\"2026-04-20T05:01:54.5616531+00:00\"", [nameof(EntityBase.Id)] = null
            });

        var method = typeof(PaginationExtensions)
            .GetMethod("BuildSeekPredicate", BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(typeof(TestEntity));

        var exception = Record.Exception(() => method.Invoke(null, [fields, payload]));

        exception.ShouldBeNull();
    }

    private sealed class TestEntity : EntityBase
    {
        public DateTimeOffset UploadedAt { get; init; }
    }
}
