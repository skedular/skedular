namespace Enterprise.Shared.UnitTests.StringExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ToSafeStringShould
{
    [Fact]
    public void Return_empty_string_when_null() =>
        ((string?)null).ToSafeString().ShouldBe(string.Empty);

    [Fact]
    public void Return_empty_string_when_whitespace() =>
        "   ".ToSafeString().ShouldBe(string.Empty);

    [Theory]
    [AutoFakeItEasyData]
    public void Return_original_string_when_not_empty(string value) =>
        value.ToSafeString().ShouldBe(value);
}
