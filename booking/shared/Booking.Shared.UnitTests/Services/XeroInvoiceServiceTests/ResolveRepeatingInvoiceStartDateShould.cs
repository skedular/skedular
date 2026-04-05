using System.Reflection;
using Booking.Shared.Services;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.UnitTests.Services.XeroInvoiceServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ResolveRepeatingInvoiceStartDateShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_Recurring_Booking_Start_Date(
        DateTimeOffset startDate)
    {
        var recurringBooking = new RecurringBookingEntity { StartDate = startDate };

        Invoke(recurringBooking).ShouldBe(startDate.UtcDateTime.Date);
    }

    private static DateTime Invoke(RecurringBookingEntity recurringBooking) =>
        (DateTime)(typeof(XeroInvoiceService)
            .GetMethod("ResolveRepeatingInvoiceStartDate", BindingFlags.Static | BindingFlags.NonPublic)!
            .Invoke(null, [recurringBooking]) ?? default(DateTime));
}
