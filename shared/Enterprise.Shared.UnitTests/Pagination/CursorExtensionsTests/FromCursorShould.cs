using Enterprise.Shared.Pagination;

namespace Enterprise.Shared.UnitTests.Pagination.CursorExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class FromCursorShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_original_cursor_value(string cursor) =>
        cursor.FromCursor().ShouldBe(cursor);
}
