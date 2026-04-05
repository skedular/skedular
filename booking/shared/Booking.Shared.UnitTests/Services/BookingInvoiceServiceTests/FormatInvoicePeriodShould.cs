using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.BookingInvoiceServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class FormatInvoicePeriodShould
{
    [Fact]
    public void Return_Date_Only_When_From_And_Until_Are_Both_At_Midnight()
    {
        var from = new DateTimeOffset(2026, 4, 6, 0, 0, 0, TimeSpan.Zero);
        var until = new DateTimeOffset(2026, 5, 6, 0, 0, 0, TimeSpan.Zero);

        var result = BookingInvoiceService.FormatInvoicePeriod(from, until);

        result.ShouldBe("06 April 2026 - 06 May 2026");
    }

    [Fact]
    public void Return_Date_And_Time_When_The_Period_Is_Not_Date_Only()
    {
        var from = new DateTimeOffset(2026, 4, 6, 9, 0, 0, TimeSpan.Zero);
        var until = new DateTimeOffset(2026, 4, 6, 17, 0, 0, TimeSpan.Zero);

        var result = BookingInvoiceService.FormatInvoicePeriod(from, until);

        result.ShouldBe("06 April 2026 09:00 - 06 April 2026 17:00");
    }
}
