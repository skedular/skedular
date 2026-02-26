using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Services;
using Shouldly;
using Testing.Shared;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.UnitTests.Services.RecurringBookingScheduleServiceTests;

public class GetReconciliationPlanShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_Missing_Days(RecurringBookingScheduleService sut, DateTimeOffset from)
    {
        var normalizedFrom = new DateTimeOffset(from.UtcDateTime.Date, TimeSpan.Zero);
        var until = normalizedFrom.AddDays(4);
        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Daily,
            RecurringBookingEndTypeConstants.Never,
            normalizedFrom,
            1);
        var existingBookings = new List<BookingEntity> { CreateBooking("b1", normalizedFrom, normalizedFrom.AddHours(1)) };

        var result = sut.GetReconciliationPlan(recurringBooking, normalizedFrom, until, existingBookings);

        result.RequiredBookingDays.Count.ShouldBe(4);
        result.MissingBookingDays.Count.ShouldBe(3);
        result.BookingsToRemove.ShouldBeEmpty();
        result.BookingsToUpdate.ShouldBeEmpty();
        result.HasMoreRequiredBookingDays.ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Duplicate_And_Obsolete_Bookings_To_Remove(RecurringBookingScheduleService sut, DateTimeOffset from)
    {
        var normalizedFrom = new DateTimeOffset(from.UtcDateTime.Date, TimeSpan.Zero);
        var until = normalizedFrom.AddDays(5);
        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Daily,
            RecurringBookingEndTypeConstants.UntilDate,
            normalizedFrom,
            1,
            endDate: normalizedFrom.AddDays(1));
        var existingBookings = new List<BookingEntity>
        {
            CreateBooking("keep-day0", normalizedFrom, normalizedFrom.AddHours(1)),
            CreateBooking("keep-day1", normalizedFrom.AddDays(1), normalizedFrom.AddDays(1).AddHours(1)),
            CreateBooking("dup-day1", normalizedFrom.AddDays(1).AddMinutes(30), normalizedFrom.AddDays(1).AddHours(2)),
            CreateBooking("obsolete-day3", normalizedFrom.AddDays(3), normalizedFrom.AddDays(3).AddHours(1))
        };

        var result = sut.GetReconciliationPlan(recurringBooking, normalizedFrom, until, existingBookings);
        var removedIds = result.BookingsToRemove.Select(item => item.Id).ToHashSet();

        removedIds.SetEquals(["dup-day1", "obsolete-day3"]).ShouldBeTrue();
        result.BookingsToUpdate.ShouldBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Booking_To_Update_When_Recurring_Time_Has_Changed(RecurringBookingScheduleService sut, DateTimeOffset from)
    {
        var normalizedFrom = new DateTimeOffset(from.UtcDateTime.Date, TimeSpan.Zero);
        var until = normalizedFrom.AddDays(2);
        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Daily,
            RecurringBookingEndTypeConstants.Never,
            normalizedFrom,
            1);
        var existingBookings = new List<BookingEntity> { CreateBooking("update-me", normalizedFrom.AddHours(2), normalizedFrom.AddHours(3)) };

        var result = sut.GetReconciliationPlan(recurringBooking, normalizedFrom, until, existingBookings);

        result.BookingsToUpdate.Select(item => item.Id).ShouldBe(["update-me"]);
        result.BookingsToRemove.ShouldBeEmpty();
        result.MissingBookingDays.Count.ShouldBe(1);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Ignore_Booking_With_Recurring_Instance_Overrides_When_Detecting_Updates_And_Removals(RecurringBookingScheduleService sut,
        DateTimeOffset from)
    {
        var normalizedFrom = new DateTimeOffset(from.UtcDateTime.Date, TimeSpan.Zero);
        var until = normalizedFrom.AddDays(2);
        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Daily,
            RecurringBookingEndTypeConstants.UntilDate,
            normalizedFrom,
            1,
            endDate: normalizedFrom);
        var existingBookings = new List<BookingEntity>
        {
            CreateBooking(
                "customized-instance",
                normalizedFrom.AddDays(1).AddHours(4),
                normalizedFrom.AddDays(1).AddHours(5),
                true)
        };

        var result = sut.GetReconciliationPlan(recurringBooking, normalizedFrom, until, existingBookings);

        result.BookingsToUpdate.ShouldBeEmpty();
        result.BookingsToRemove.ShouldBeEmpty();
        result.MissingBookingDays.ShouldContain(DateOnly.FromDateTime(normalizedFrom.UtcDateTime.Date));
    }

    private static RecurringBookingEntity CreateRecurringBooking(
        string frequency,
        string endType,
        DateTimeOffset startDate,
        int interval,
        ICollection<string>? byWeekDays = null,
        int? byMonthDay = null,
        int? bySetPosition = null,
        DateTimeOffset? endDate = null,
        int? occurrenceCount = null) =>
        new()
        {
            Category = BookingCategoryConstants.WorkingFromOffice,
            Channel = BookingChannelConstants.Private,
            From = startDate,
            Until = startDate.AddHours(1),
            Frequency = frequency,
            EndType = endType,
            StartDate = startDate,
            Interval = interval,
            ByMonthDay = byMonthDay,
            BySetPosition = bySetPosition,
            ByWeekDays = byWeekDays ?? [],
            EndDate = endDate,
            OccurrenceCount = occurrenceCount,
            SkippedDates = []
        };

    private static BookingEntity CreateBooking(
        string id,
        DateTimeOffset from,
        DateTimeOffset until,
        bool? hasRecurringInstanceOverrides = null) =>
        new()
        {
            Id = id,
            From = from,
            Until = until,
            Category = BookingCategoryConstants.WorkingFromOffice,
            Channel = BookingChannelConstants.Private,
            Schedules = [],
            HasRecurringInstanceOverrides = hasRecurringInstanceOverrides
        };
}
