namespace Enterprise.Shared.UnitTests.Extensions.DecimalExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ToNullDoubleShould
{
    [Fact]
    public void Return_double_min_value_when_null() =>
        ((decimal?)null).ToNullDouble().ShouldBe(double.MinValue);

    [Theory]
    [InlineData(0.0)]
    [InlineData(1.5)]
    [InlineData(-99.99)]
    public void Return_converted_double_when_not_null(double expected)
    {
        var input = (decimal?)Convert.ToDecimal(expected);
        input.ToNullDouble().ShouldBe(Convert.ToDouble(input));
    }
}
