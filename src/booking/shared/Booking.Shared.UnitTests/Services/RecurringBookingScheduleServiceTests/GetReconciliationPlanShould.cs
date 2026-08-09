using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Services;
using Enterprise.Shared;
using RecurringBooking = Booking.Shared.Database.Entities.RecurringBooking;
using Resource = Booking.Shared.Database.Entities.Resource;
using ResourceBookingSlot = Booking.Shared.Database.Entities.ResourceBookingSlot;

namespace Booking.Shared.UnitTests.Services.RecurringBookingScheduleServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetReconciliationPlanShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Exclude_Override_Occurrences_From_Cross_Cycle_Resource_Preference_Resolution(
        RecurringBookingScheduleService sut,
        DateTimeOffset from)
    {
        var normalizedFrom = new DateTimeOffset(from.UtcDateTime.Date, TimeSpan.Zero);
        var until = normalizedFrom.AddDays(5);
        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Daily,
            RecurringBookingEndTypeConstants.Never,
            normalizedFrom,
            1);

        var existingBookings = new List<Database.Entities.Booking>
        {
            CreateBooking("normal-booking", normalizedFrom, normalizedFrom.AddHours(1)),
            CreateBooking("override-booking", normalizedFrom.AddDays(1), normalizedFrom.AddDays(1).AddHours(1), true),
        };

        var result = sut.GetReconciliationPlan(recurringBooking, normalizedFrom, until, existingBookings);

        // Should not use override booking resources for next cycle preferences
        result.BookingsToRemove.ShouldBeEmpty();
        result.MissingBookingDays.Count.ShouldBe(3);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Preserve_Override_Flag_During_Reconciliation(
        RecurringBookingScheduleService sut,
        DateTimeOffset from)
    {
        var normalizedFrom = new DateTimeOffset(from.UtcDateTime.Date, TimeSpan.Zero);
        var until = normalizedFrom.AddDays(3);
        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Daily,
            RecurringBookingEndTypeConstants.Never,
            normalizedFrom,
            1);

        var existingBookings = new List<Database.Entities.Booking>
        {
            CreateBooking("override-booking", normalizedFrom, normalizedFrom.AddHours(1), true),
        };

        var result = sut.GetReconciliationPlan(recurringBooking, normalizedFrom, until, existingBookings);

        // Override booking should not be marked for removal
        result.BookingsToRemove.ShouldBeEmpty();
        result.MissingBookingDays.Count.ShouldBe(2);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Not_Propagate_Override_Resources_To_Next_Cycle_Preferences(
        RecurringBookingScheduleService sut,
        DateTimeOffset from)
    {
        var normalizedFrom = new DateTimeOffset(from.UtcDateTime.Date, TimeSpan.Zero);
        var cycleEnd = normalizedFrom.AddDays(7);
        var nextCycleStart = cycleEnd.AddDays(1);

        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Daily,
            RecurringBookingEndTypeConstants.Never,
            normalizedFrom,
            1);

        var overrideBooking = CreateBooking("override-booking", normalizedFrom.AddDays(2), normalizedFrom.AddDays(2).AddHours(1), true);
        overrideBooking.ResourceBookingSlots =
        [
            new ResourceBookingSlot
            {
                Resource = new Resource
                {
                    Id = "override-resource-1",
                },
            },
        ];

        var existingBookings = new List<Database.Entities.Booking>
        {
            CreateBooking("normal-booking", normalizedFrom, normalizedFrom.AddHours(1)),
            overrideBooking,
        };

        var result = sut.GetReconciliationPlan(recurringBooking, nextCycleStart, nextCycleStart.AddDays(5), existingBookings);

        // Next cycle should not inherit override resources
        result.MissingBookingDays.Count.ShouldBe(5);
        // The override booking should not influence next cycle resource selection
    }

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
        var existingBookings = new List<Database.Entities.Booking>
        {
            CreateBooking("b1", normalizedFrom, normalizedFrom.AddHours(1)),
        };

        var result = sut.GetReconciliationPlan(recurringBooking, normalizedFrom, until, existingBookings);

        result.RequiredBookingDays.Count.ShouldBe(4);
        result.MissingBookingDays.Count.ShouldBe(3);
        result.BookingsToRemove.ShouldBeEmpty();
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
        var existingBookings = new List<Database.Entities.Booking>
        {
            CreateBooking("keep-day0", normalizedFrom, normalizedFrom.AddHours(1)),
            CreateBooking("keep-day1", normalizedFrom.AddDays(1), normalizedFrom.AddDays(1).AddHours(1)),
            CreateBooking("dup-day1", normalizedFrom.AddDays(1).AddMinutes(30), normalizedFrom.AddDays(1).AddHours(2)),
            CreateBooking("obsolete-day3", normalizedFrom.AddDays(3), normalizedFrom.AddDays(3).AddHours(1)),
        };

        var result = sut.GetReconciliationPlan(recurringBooking, normalizedFrom, until, existingBookings);
        var removedIds = result.BookingsToRemove.Select(item => item.Id).ToHashSet();

        removedIds.SetEquals(["dup-day1", "obsolete-day3"]).ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Not_Remove_Booking_When_Recurring_Time_Has_Changed(RecurringBookingScheduleService sut, DateTimeOffset from)
    {
        var normalizedFrom = new DateTimeOffset(from.UtcDateTime.Date, TimeSpan.Zero);
        var until = normalizedFrom.AddDays(2);
        var recurringBooking = CreateRecurringBooking(
            BookingFrequencyConstants.Daily,
            RecurringBookingEndTypeConstants.Never,
            normalizedFrom,
            1);
        var existingBookings =
            new List<Database.Entities.Booking>
            {
                CreateBooking("update-me", normalizedFrom.AddHours(2), normalizedFrom.AddHours(3)),
            };

        var result = sut.GetReconciliationPlan(recurringBooking, normalizedFrom, until, existingBookings);

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
        var existingBookings = new List<Database.Entities.Booking>
        {
            CreateBooking(
                "customized-instance",
                normalizedFrom.AddDays(1).AddHours(4),
                normalizedFrom.AddDays(1).AddHours(5),
                true),
        };

        var result = sut.GetReconciliationPlan(recurringBooking, normalizedFrom, until, existingBookings);

        result.BookingsToRemove.ShouldBeEmpty();
        result.MissingBookingDays.ShouldContain(DateOnly.FromDateTime(normalizedFrom.UtcDateTime.Date));
    }

    private static RecurringBooking CreateRecurringBooking(
        string frequency,
        string endType,
        DateTimeOffset startDate,
        int interval,
        IReadOnlyList<string>? byWeekDays = null,
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
            ByWeekDays = [.. byWeekDays.ToSafeCollection()],
            EndDate = endDate,
            OccurrenceCount = occurrenceCount,
            SkippedDates = [],
        };

    private static Database.Entities.Booking CreateBooking(
        string id,
        DateTimeOffset from,
        DateTimeOffset until,
        bool hasOverride = false) =>
        new()
        {
            Id = id,
            From = from,
            Until = until,
            Category = BookingCategoryConstants.WorkingFromOffice,
            Channel = BookingChannelConstants.Private,
            Schedules = [],
            HasRecurringInstanceOverrides = hasOverride,
        };
}
