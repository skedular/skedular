namespace Enterprise.Shared.UnitTests.ExtensionsTests.DecimalExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RoundedDecimalShould
{
    [Theory]
    [InlineData(1.555, 1.56)]
    [InlineData(1.554, 1.55)]
    [InlineData(100, 100)]
    public void Round_to_two_decimal_places(decimal input, decimal expected) =>
        input.RoundedDecimal().ShouldBe(expected);
}
