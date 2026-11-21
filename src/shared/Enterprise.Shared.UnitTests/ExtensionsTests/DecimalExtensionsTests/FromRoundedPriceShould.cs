using System.Globalization;

namespace Enterprise.Shared.UnitTests.ExtensionsTests.DecimalExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class FromRoundedPriceShould
{
    [Fact]
    public void Parse_decimal_with_invariant_culture_regardless_of_current_culture()
    {
        var originalCulture = CultureInfo.CurrentCulture;

        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

            "1.50".FromRoundedPrice().ShouldBe(1.5m);
            "1.50".FromRoundedDecimal().ShouldBe(1.5m);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Theory]
    [InlineData("1.50", 1.5)]
    [InlineData("100.00", 100)]
    public void Parse_decimal_from_string(string input, decimal expected)
    {
        input.FromRoundedPrice().ShouldBe(expected);
        input.FromRoundedDecimal().ShouldBe(expected);
    }
}
