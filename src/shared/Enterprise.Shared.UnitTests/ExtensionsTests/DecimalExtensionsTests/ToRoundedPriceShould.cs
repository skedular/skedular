using System.Globalization;

namespace Enterprise.Shared.UnitTests.ExtensionsTests.DecimalExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ToRoundedPriceShould
{
    [Fact]
    public void Format_decimal_with_invariant_culture_regardless_of_current_culture()
    {
        var originalCulture = CultureInfo.CurrentCulture;

        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

            1.5m.ToRoundedPrice().ShouldBe("1.50");
            1.5m.ToRoundedDecimal().ShouldBe("1.50");
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Theory]
    [InlineData(1.5, "1.50")]
    [InlineData(100, "100.00")]
    [InlineData(0.999, "1.00")]
    public void Format_to_two_decimal_places(decimal input, string expected) =>
        input.ToRoundedPrice().ShouldBe(expected);

    [Theory]
    [InlineData(1.5, "1.50")]
    [InlineData(100, "100.00")]
    [InlineData(0.999, "1.00")]
    public void ToRoundedDecimal_formats_same_as_ToRoundedPrice(decimal input, string expected) =>
        input.ToRoundedDecimal().ShouldBe(expected);
}
