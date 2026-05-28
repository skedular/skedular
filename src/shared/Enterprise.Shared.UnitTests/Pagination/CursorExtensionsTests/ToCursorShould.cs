using Enterprise.Shared.Pagination;

namespace Enterprise.Shared.UnitTests.Pagination.CursorExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ToCursorShould
{
    [Fact]
    public void Return_empty_string_when_null() =>
        ((string?)null).ToCursor().ShouldBe(string.Empty);

    [Fact]
    public void Return_empty_string_when_whitespace() =>
        "   ".ToCursor().ShouldBe(string.Empty);

    [Theory]
    [AutoFakeItEasyData]
    public void Return_original_value_when_not_empty(string cursor) =>
        cursor.ToCursor().ShouldBe(cursor);
}
