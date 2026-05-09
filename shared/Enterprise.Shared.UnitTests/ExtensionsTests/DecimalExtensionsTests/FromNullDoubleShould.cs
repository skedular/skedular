namespace Enterprise.Shared.UnitTests.ExtensionsTests.DecimalExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class FromNullDoubleShould
{
    [Fact]
    public void Return_null_when_double_min_value() =>
        double.MinValue.FromNullDouble().ShouldBeNull();

    [Theory]
    [InlineData(0.0)]
    [InlineData(1.5)]
    [InlineData(-99.99)]
    public void Return_decimal_when_not_double_min_value(double input)
    {
        var result = input.FromNullDouble();
        result.ShouldNotBeNull();
        result.ShouldBe(Convert.ToDecimal(input));
    }
}
