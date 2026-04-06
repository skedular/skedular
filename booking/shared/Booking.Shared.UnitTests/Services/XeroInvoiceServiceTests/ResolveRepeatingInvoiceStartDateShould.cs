using System.Reflection;
using Booking.Shared.Services;
using Xero.NetStandard.OAuth2.Model.Accounting;
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

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Next_Billing_Boundary_When_Initial_Invoice_Is_Created_Immediately(
        DateTimeOffset startDate)
    {
        var recurringBooking = new RecurringBookingEntity { StartDate = startDate };
        var scheduleDefinition = new XeroRepeatingInvoiceScheduleDefinition(
            "OrganizationBillingCycle",
            Schedule.UnitEnum.MONTHLY,
            1,
            100m);

        Invoke(recurringBooking, scheduleDefinition, true).ShouldBe(startDate.UtcDateTime.Date.AddMonths(1));
    }

    private static DateTime Invoke(RecurringBookingEntity recurringBooking) =>
        (DateTime)(typeof(XeroInvoiceService)
            .GetMethod(
                "ResolveRepeatingInvoiceStartDate",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                [typeof(RecurringBookingEntity)],
                null)!
            .Invoke(null, [recurringBooking]) ?? default(DateTime));

    private static DateTime Invoke(
        RecurringBookingEntity recurringBooking,
        XeroRepeatingInvoiceScheduleDefinition scheduleDefinition,
        bool shouldCreateInitialInvoiceImmediately) =>
        (DateTime)(typeof(XeroInvoiceService)
            .GetMethod(
                "ResolveRepeatingInvoiceStartDate",
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                [typeof(RecurringBookingEntity), typeof(XeroRepeatingInvoiceScheduleDefinition), typeof(bool)],
                null)!
            .Invoke(null, [recurringBooking, scheduleDefinition, shouldCreateInitialInvoiceImmediately]) ?? default(DateTime));
}
