using Booking.Shared.Models;

namespace Booking.Shared.UnitTests.Models;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceRefundQuoteShould
{
    [Fact]
    public void Calculate_Away_From_Zero_To_Two_Decimal_Places()
    {
        var quote = new MarketplaceRefundQuote(true, true, 33, 60);

        quote.CalculateRefundAmount(10.01m).ShouldBe(3.30m);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(100, 125.50)]
    public void Calculate_Zero_Or_Full_Refunds(int percentage, decimal expected)
    {
        var quote = new MarketplaceRefundQuote(true, percentage > 0, percentage, 0);

        quote.CalculateRefundAmount(125.50m).ShouldBe(expected);
    }
}
