using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Services;
using Shouldly;
using Testing.Shared;
using RecurringBooking = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.UnitTests.Services.RecurringBookingScheduleServiceTests;

public class GetRequiredBookingDaysShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_Expected_Days_For_Daily_Recurrence(RecurringBookingScheduleService sut, DateTimeOffset from, int intervalSeed)
    {
        var until = from.AddMonths(1);
        var interval = Math.Abs(intervalSeed % 4) + 1;
        var recurringBooking = CreateRecurringBooking(BookingFrequencyConstants.Daily, RecurringBookingEndTypeConstants.Never, from, interval);
        var expectedDays = GetDailyExpectedDays(from, until, DateOnly.FromDateTime(from.UtcDateTime.Date), interval);
        var requiredDays = sut.GetRequiredBookingDays(recurringBooking, from, until).Days;

        requiredDays.ToHashSet().SetEquals(expectedDays.ToHashSet()).ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Only_Configured_Week_Days_For_Weekly_Recurrence(RecurringBookingScheduleService sut, DateTimeOffset from, int intervalSeed)
    {
        var until = from.AddMonths(1);
        var startDate = from.AddDays(-14);
        var startDay = startDate.DayOfWeek;
        var secondDay = (DayOfWeek)(((int)startDay + 2) % 7);

        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Weekly,
            RecurringBookingEndTypeConstants.Never,
            startDate,
            Math.Abs(intervalSeed % 3) + 1,
            [startDay.ToDayOfWeek(), secondDay.ToDayOfWeek()]);

        var requiredDays = sut.GetRequiredBookingDays(recurringBooking, from, until).Days;

        requiredDays.ShouldNotBeEmpty();
        requiredDays.All(day => new[] { startDay, secondDay }.Contains(day.DayOfWeek)).ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Default_To_Start_Day_Of_Week_When_Weekly_ByWeekDays_Is_Empty(RecurringBookingScheduleService sut, DateTimeOffset from)
    {
        var until = from.AddMonths(1);
        var startDate = from.AddDays(-7);
        var recurringBooking = CreateRecurringBooking(BookingFrequencyConstants.Weekly, RecurringBookingEndTypeConstants.Never, startDate, 1, []);
        var requiredDays = sut.GetRequiredBookingDays(recurringBooking, from, until).Days;

        requiredDays.ShouldNotBeEmpty();
        requiredDays.All(day => day.DayOfWeek == startDate.DayOfWeek).ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Exclude_Skipped_Dates_From_Missing_Days(RecurringBookingScheduleService sut, DateTimeOffset from, int skipOffsetSeed)
    {
        var until = from.AddMonths(1);
        var recurringBooking = CreateRecurringBooking(BookingFrequencyConstants.Daily, RecurringBookingEndTypeConstants.Never, from, 1);
        var skipOffset = Math.Abs(skipOffsetSeed % 10);
        var skippedDate = from.AddDays(skipOffset);
        recurringBooking.SkippedDates = [skippedDate];

        var requiredDays = sut.GetRequiredBookingDays(recurringBooking, from, until).Days;

        requiredDays.ShouldNotContain(DateOnly.FromDateTime(skippedDate.UtcDateTime.Date));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Respect_UntilDate_End_Type(RecurringBookingScheduleService sut, DateTimeOffset from, int daysUntilEndSeed)
    {
        var until = from.AddMonths(1);
        var endDate = from.AddDays(Math.Abs(daysUntilEndSeed % 12) + 2);
        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Daily,
            RecurringBookingEndTypeConstants.UntilDate,
            from,
            1,
            endDate: endDate);
        var requiredDays = sut.GetRequiredBookingDays(recurringBooking, from, until).Days;

        requiredDays.ShouldNotBeEmpty();
        requiredDays.All(day => day <= DateOnly.FromDateTime(endDate.UtcDateTime.Date)).ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Respect_AfterOccurrences_End_Type(RecurringBookingScheduleService sut, DateTimeOffset from, int occurrenceCountSeed)
    {
        var until = from.AddMonths(1);
        var occurrenceCount = Math.Abs(occurrenceCountSeed % 8) + 1;
        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Daily,
            RecurringBookingEndTypeConstants.AfterOccurrences,
            from,
            1,
            occurrenceCount: occurrenceCount);
        var requiredDays = sut.GetRequiredBookingDays(recurringBooking, from, until).Days;

        requiredDays.Count.ShouldBe(occurrenceCount);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_At_Most_One_Monthly_Day_Within_One_Month_Window(RecurringBookingScheduleService sut, DateTimeOffset from, int startDaySeed)
    {
        var until = from.AddMonths(1);
        var safeDay = Math.Abs(startDaySeed % 28) + 1;
        var startDate = new DateTimeOffset(from.Year, from.Month, safeDay, 0, 0, 0, TimeSpan.Zero);
        var recurringBooking = CreateRecurringBooking(BookingFrequencyConstants.Monthly, RecurringBookingEndTypeConstants.Never, startDate, 1);
        var requiredDays = sut.GetRequiredBookingDays(recurringBooking, from, until).Days;

        requiredDays.Count.ShouldBeInRange(0, 1);
        requiredDays.All(day => day.Day == safeDay).ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Use_ByMonthDay_For_Monthly_Recurrence(RecurringBookingScheduleService sut, DateTimeOffset seed, int byMonthDaySeed)
    {
        var from = new DateTimeOffset(seed.UtcDateTime.Year, seed.UtcDateTime.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var until = from.AddMonths(1);
        var daysInMonth = DateTime.DaysInMonth(from.Year, from.Month);
        var byMonthDay = Math.Abs(byMonthDaySeed % daysInMonth) + 1;
        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Monthly,
            RecurringBookingEndTypeConstants.Never,
            from.AddMonths(-1),
            1,
            byMonthDay: byMonthDay);
        var requiredDays = sut.GetRequiredBookingDays(recurringBooking, from, until).Days;

        requiredDays.ShouldHaveSingleItem();
        requiredDays.Single().Day.ShouldBe(byMonthDay);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Use_Negative_ByMonthDay_As_Day_From_End_Of_Month(RecurringBookingScheduleService sut, DateTimeOffset seed)
    {
        var from = new DateTimeOffset(seed.UtcDateTime.Year, seed.UtcDateTime.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var until = from.AddMonths(1);
        var expectedDay = DateTime.DaysInMonth(from.Year, from.Month);
        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Monthly,
            RecurringBookingEndTypeConstants.Never,
            from.AddMonths(-1),
            1,
            byMonthDay: -1);
        var requiredDays = sut.GetRequiredBookingDays(recurringBooking, from, until).Days;

        requiredDays.ShouldHaveSingleItem();
        requiredDays.Single().Day.ShouldBe(expectedDay);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Use_BySetPosition_For_Monthly_Recurrence(RecurringBookingScheduleService sut, DateTimeOffset seed, int setPositionSeed)
    {
        var from = new DateTimeOffset(seed.UtcDateTime.Year, seed.UtcDateTime.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var until = from.AddMonths(1);
        var bySetPosition = Math.Abs(setPositionSeed % 4) + 1;
        var dayOfWeek = from.DayOfWeek;
        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Monthly,
            RecurringBookingEndTypeConstants.Never,
            from.AddMonths(-1),
            1,
            [dayOfWeek.ToDayOfWeek()],
            bySetPosition: bySetPosition);
        var requiredDays = sut.GetRequiredBookingDays(recurringBooking, from, until).Days;

        requiredDays.ShouldHaveSingleItem();
        requiredDays.Single().DayOfWeek.ShouldBe(dayOfWeek);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_HasMoreRequiredBookingDays_As_False_When_UntilDate_Is_In_Past(RecurringBookingScheduleService sut, DateTimeOffset from)
    {
        var normalizedFrom = new DateTimeOffset(from.UtcDateTime.Date, TimeSpan.Zero);
        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Daily,
            RecurringBookingEndTypeConstants.UntilDate,
            normalizedFrom.AddDays(-10),
            1,
            endDate: normalizedFrom.AddDays(-1));

        var result = sut.GetRequiredBookingDays(recurringBooking, normalizedFrom, normalizedFrom.AddMonths(1));

        result.Days.ShouldBeEmpty();
        result.HasMoreRequiredBookingDays.ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_HasMoreRequiredBookingDays_As_False_When_AfterOccurrences_Already_Exhausted(
        RecurringBookingScheduleService sut,
        DateTimeOffset from)
    {
        var normalizedFrom = new DateTimeOffset(from.UtcDateTime.Date, TimeSpan.Zero);
        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Daily,
            RecurringBookingEndTypeConstants.AfterOccurrences,
            normalizedFrom.AddDays(-10),
            1,
            occurrenceCount: 3);

        var result = sut.GetRequiredBookingDays(recurringBooking, normalizedFrom, normalizedFrom.AddMonths(1));

        result.Days.ShouldBeEmpty();
        result.HasMoreRequiredBookingDays.ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_HasMoreRequiredBookingDays_As_False_When_UntilDate_Is_Inside_Current_Window(
        RecurringBookingScheduleService sut,
        DateTimeOffset from)
    {
        var normalizedFrom = new DateTimeOffset(from.UtcDateTime.Date, TimeSpan.Zero);
        var normalizedUntil = normalizedFrom.AddMonths(1);
        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Weekly,
            RecurringBookingEndTypeConstants.UntilDate,
            normalizedFrom,
            1,
            [DayOfWeek.Monday.ToDayOfWeek(), DayOfWeek.Tuesday.ToDayOfWeek(), DayOfWeek.Friday.ToDayOfWeek()],
            endDate: normalizedFrom.AddDays(10));

        var result = sut.GetRequiredBookingDays(recurringBooking, normalizedFrom, normalizedUntil);

        result.Days.ShouldNotBeEmpty();
        result.HasMoreRequiredBookingDays.ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_HasMoreRequiredBookingDays_As_True_When_UntilDate_Is_After_Current_Window(
        RecurringBookingScheduleService sut,
        DateTimeOffset from)
    {
        var normalizedFrom = new DateTimeOffset(from.UtcDateTime.Date, TimeSpan.Zero);
        var normalizedUntil = normalizedFrom.AddMonths(1);
        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Weekly,
            RecurringBookingEndTypeConstants.UntilDate,
            normalizedFrom,
            1,
            [DayOfWeek.Monday.ToDayOfWeek(), DayOfWeek.Tuesday.ToDayOfWeek(), DayOfWeek.Friday.ToDayOfWeek()],
            endDate: normalizedUntil.AddDays(10));

        var result = sut.GetRequiredBookingDays(recurringBooking, normalizedFrom, normalizedUntil);

        result.Days.ShouldNotBeEmpty();
        result.HasMoreRequiredBookingDays.ShouldBeTrue();
    }

    private static List<DateOnly> GetDailyExpectedDays(DateTimeOffset from, DateTimeOffset until, DateOnly recurrenceStart, int interval)
    {
        var expectedDays = new List<DateOnly>();
        var windowStart = DateOnly.FromDateTime(from.UtcDateTime.Date);
        var windowEndExclusive = DateOnly.FromDateTime(until.UtcDateTime.Date);

        for (var cursor = windowStart; cursor < windowEndExclusive; cursor = cursor.AddDays(1))
        {
            if ((cursor.DayNumber - recurrenceStart.DayNumber) % interval == 0)
            {
                expectedDays.Add(cursor);
            }
        }

        return expectedDays;
    }

    private static RecurringBooking CreateRecurringBooking(
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
}
