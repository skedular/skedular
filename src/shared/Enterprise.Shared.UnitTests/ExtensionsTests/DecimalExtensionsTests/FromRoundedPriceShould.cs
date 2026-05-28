namespace Enterprise.Shared.UnitTests.ExtensionsTests.DecimalExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class FromRoundedPriceShould
{
    [Theory]
    [InlineData("1.50", 1.5)]
    [InlineData("100.00", 100)]
    public void Parse_decimal_from_string(string input, decimal expected)
    {
        input.FromRoundedPrice().ShouldBe(expected);
        input.FromRoundedDecimal().ShouldBe(expected);
    }
}
