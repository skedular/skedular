using Api.Shared.Services.Models;
using Booking.Shared.Models;
using RecurringBooking = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.Services;

public record RecurringBookingReconciliationPlan(
    IReadOnlyList<DateOnly> RequiredBookingDays,
    IReadOnlyList<DateOnly> MissingBookingDays,
    IReadOnlyList<Database.Entities.Booking> BookingsToRemove,
    bool HasMoreRequiredBookingDays);

/// <summary>
///     Provides recurring-schedule utilities for any recurring booking type.
/// </summary>
public interface IRecurringBookingScheduleService
{
    /// <summary>
    ///     Produces a reusable reconciliation plan for recurring bookings in a given window.
    /// </summary>
    RecurringBookingReconciliationPlan GetReconciliationPlan(
        RecurringBooking recurringBooking,
        DateTimeOffset from,
        DateTimeOffset until,
        IReadOnlyList<Database.Entities.Booking> existingBookings);
}

/// <summary>
///     Computes required recurrence days for a given window.
/// </summary>
public class RecurringBookingScheduleService : IRecurringBookingScheduleService
{
    public RecurringBookingReconciliationPlan GetReconciliationPlan(
        RecurringBooking recurringBooking,
        DateTimeOffset from,
        DateTimeOffset until,
        IReadOnlyList<Database.Entities.Booking> existingBookings)
    {
        // Compute required days in the requested planning horizon.
        var requiredBookingDaysResponse = GetRequiredBookingDays(recurringBooking, from, until);
        var requiredBookingDays = requiredBookingDaysResponse.Days.ToHashSet();

        // Required days without any existing booking are candidates to add.
        // Customized instances still count as existing for this check, so we do not create duplicates.
        var existingBookingDays = existingBookings.Select(booking => DateOnly.FromDateTime(booking.From.UtcDateTime.Date)).ToHashSet();
        var missingBookingDays = requiredBookingDays.Where(day => !existingBookingDays.Contains(day)).ToList();

        // Existing bookings can become stale when recurrence rules change.
        // We evaluate up to the furthest existing booking day, so we can remove
        // both obsolete days and duplicate bookings on valid days.
        var bookingsToRemove = new List<Database.Entities.Booking>();
        if (existingBookings.Count > 0)
        {
            var maxBookingDay = existingBookings.Select(booking => DateOnly.FromDateTime(booking.From.UtcDateTime.Date)).Max();
            var evaluationUntil = new DateTimeOffset(maxBookingDay.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), TimeSpan.Zero);
            var expectedBookingDays = GetRequiredBookingDays(recurringBooking, from, evaluationUntil).Days.ToHashSet();
            var groupedByDay = existingBookings.GroupBy(booking => DateOnly.FromDateTime(booking.From.UtcDateTime.Date));

            foreach (var dayGroup in groupedByDay)
            {
                // Customized instances are intentionally not touched by recurrence reconciliation.
                var regularInstances = dayGroup
                    .Where(booking => booking.HasRecurringInstanceOverrides != true)
                    .ToList();
                if (regularInstances.Count == 0)
                {
                    continue;
                }

                if (!expectedBookingDays.Contains(dayGroup.Key))
                {
                    bookingsToRemove.AddRange(regularInstances);

                    continue;
                }

                var orderedRegularInstances = regularInstances.OrderBy(booking => booking.From).ToList();

                // Keep one regular instance and remove duplicate regular instances for the same day.
                bookingsToRemove.AddRange(orderedRegularInstances.Skip(1));
            }
        }

        return new RecurringBookingReconciliationPlan(
            requiredBookingDays.ToList(),
            missingBookingDays,
            bookingsToRemove,
            requiredBookingDaysResponse.HasMoreRequiredBookingDays);
    }

    private static bool IsRecurringOnDate(RecurringBooking recurringBooking, DateOnly recurrenceStart, DateOnly date, int interval)
    {
        // No match before recurrence start.
        if (date < recurrenceStart)
        {
            return false;
        }

        return recurringBooking.Frequency.ToBookingFrequency() switch
        {
            BookingFrequency.Daily => IsDailyRecurringOnDate(recurrenceStart, date, interval),
            BookingFrequency.Weekly => IsWeeklyRecurringOnDate(recurringBooking, recurrenceStart, date, interval),
            BookingFrequency.Monthly => IsMonthlyRecurringOnDate(recurringBooking, recurrenceStart, date, interval),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static bool IsDailyRecurringOnDate(DateOnly recurrenceStart, DateOnly date, int interval) =>
        (date.DayNumber - recurrenceStart.DayNumber) % interval == 0;

    private static bool IsWeeklyRecurringOnDate(RecurringBooking recurringBooking, DateOnly recurrenceStart, DateOnly date, int interval)
    {
        var byWeekDays = recurringBooking.ByWeekDays.Select(item => item.ToDayOfWeek()).ToHashSet();

        // Fallback to start weekday if none is provided.
        if (byWeekDays.Count == 0)
        {
            byWeekDays.Add(recurrenceStart.DayOfWeek);
        }

        var daysBetween = date.DayNumber - recurrenceStart.DayNumber;
        var weeksBetween = daysBetween / 7;

        return weeksBetween % interval == 0 && byWeekDays.Contains(date.DayOfWeek);
    }

    private static bool IsMonthlyRecurringOnDate(RecurringBooking recurringBooking, DateOnly recurrenceStart, DateOnly date, int interval)
    {
        var monthsBetween = (date.Year - recurrenceStart.Year) * 12 + (date.Month - recurrenceStart.Month);
        if (monthsBetween < 0 || monthsBetween % interval != 0)
        {
            return false;
        }

        // Priority 1: explicit day-of-month (supports negative-from-end values).
        if (recurringBooking.ByMonthDay.HasValue)
        {
            var byMonthDay = recurringBooking.ByMonthDay.Value;
            var dayInMonth = ResolveDayInMonth(date.Year, date.Month, byMonthDay);
            return dayInMonth.HasValue && date.Day == dayInMonth.Value;
        }

        // Priority 2: Nth weekday selection.
        if (recurringBooking.BySetPosition.HasValue)
        {
            var byWeekDays = recurringBooking.ByWeekDays.Select(item => item.ToDayOfWeek()).ToHashSet();

            // Fallback to start weekday if BYWEEKDAY is empty.
            if (byWeekDays.Count == 0)
            {
                byWeekDays.Add(recurrenceStart.DayOfWeek);
            }

            var dayInMonth = ResolveDayInMonthBySetPosition(date.Year, date.Month, byWeekDays.ToList(), recurringBooking.BySetPosition.Value);

            return dayInMonth.HasValue && date.Day == dayInMonth.Value;
        }

        // Fallback: same day-of-month as recurrence start.
        return date.Day == recurrenceStart.Day;
    }

    private static int? ResolveDayInMonth(int year, int month, int byMonthDay)
    {
        // Positive: exact day; negative: from the month end (-1 is last day).
        var daysInMonth = DateTime.DaysInMonth(year, month);
        switch (byMonthDay)
        {
            case > 0:
                return byMonthDay <= daysInMonth ? byMonthDay : null;

            case < 0:
                {
                    var dayFromEnd = daysInMonth + byMonthDay + 1;
                    return dayFromEnd >= 1 ? dayFromEnd : null;
                }

            default:
                return null;
        }
    }

    private static int? ResolveDayInMonthBySetPosition(int year, int month, IReadOnlyList<DayOfWeek> byWeekDays, int bySetPosition)
    {
        // Invalid selector inputs.
        if (bySetPosition == 0 || byWeekDays.Count == 0)
        {
            return null;
        }

        // Build ordered matching days for the month.
        var matchingDays = Enumerable
            .Range(1, DateTime.DaysInMonth(year, month))
            .Select(day => new DateOnly(year, month, day))
            .Where(day => byWeekDays.Contains(day.DayOfWeek))
            .ToList();

        if (matchingDays.Count == 0)
        {
            return null;
        }

        // Positive index is from start (1-based), negative is from end.
        var index = bySetPosition > 0 ? bySetPosition - 1 : matchingDays.Count + bySetPosition;

        return index >= 0 && index < matchingDays.Count ? matchingDays[index].Day : null;
    }

    private static bool HasAnyRequiredBookingDayOnOrAfter(
        RecurringBooking recurringBooking,
        DateOnly thresholdStart,
        DateOnly recurrenceStart,
        DateOnly? recurrenceEnd,
        HashSet<DateOnly> skippedDays,
        int interval,
        RecurringBookingEndType recurringBookingEndType,
        int? occurrenceLimit)
    {
        // A "never" recurrence is unbounded; with finite skipped days there is always a future valid booking day.
        if (recurringBookingEndType == RecurringBookingEndType.Never)
        {
            return true;
        }

        var cursor = recurrenceStart;
        var occurrenceCount = 0;

        while (true)
        {
            if (IsRecurringOnDate(recurringBooking, recurrenceStart, cursor, interval))
            {
                occurrenceCount++;

                var isAfterStart = cursor >= thresholdStart;
                var isBeforeExplicitEnd = !recurrenceEnd.HasValue || cursor <= recurrenceEnd.Value;
                var isUntilDateValid = recurringBookingEndType != RecurringBookingEndType.UntilDate || isBeforeExplicitEnd;
                var isWithinOccurrenceLimit = !occurrenceLimit.HasValue || occurrenceCount <= occurrenceLimit.Value;

                if (isAfterStart && isUntilDateValid && isWithinOccurrenceLimit && !skippedDays.Contains(cursor))
                {
                    return true;
                }

                if (occurrenceLimit.HasValue && occurrenceCount >= occurrenceLimit.Value)
                {
                    return false;
                }
            }

            if (recurringBookingEndType == RecurringBookingEndType.UntilDate && recurrenceEnd.HasValue && cursor >= recurrenceEnd.Value)
            {
                return false;
            }

            cursor = cursor.AddDays(1);
        }
    }

    /// <summary>
    ///     Calculates required calendar days in the current window and whether valid days still exist after this window.
    /// </summary>
    public (IReadOnlyList<DateOnly> Days, bool HasMoreRequiredBookingDays) GetRequiredBookingDays(
        RecurringBooking recurringBooking,
        DateTimeOffset from,
        DateTimeOffset until)
    {
        // Normalize all boundaries to DateOnly for day-based recurrence matching.
        var windowStart = DateOnly.FromDateTime(from.UtcDateTime.Date);
        var windowEndExclusive = DateOnly.FromDateTime(until.UtcDateTime.Date);
        var recurrenceStart = DateOnly.FromDateTime(recurringBooking.StartDate.UtcDateTime.Date);
        var recurrenceEnd = recurringBooking.EndDate.HasValue
            ? DateOnly.FromDateTime(recurringBooking.EndDate.Value.UtcDateTime.Date)
            : (DateOnly?)null;
        var skippedDays = recurringBooking.SkippedDates.Select(item => DateOnly.FromDateTime(item.UtcDateTime.Date)).ToHashSet();
        var interval = Math.Max(1, recurringBooking.Interval);
        var recurringBookingEndType = recurringBooking.EndType.ToRecurringBookingEndType();
        var occurrenceLimit = recurringBookingEndType == RecurringBookingEndType.AfterOccurrences ? recurringBooking.OccurrenceCount : null;
        var occurrenceCount = 0;
        var days = new HashSet<DateOnly>();
        var cursor = recurrenceStart;

        while (cursor < windowEndExclusive)
        {
            if (IsRecurringOnDate(recurringBooking, recurrenceStart, cursor, interval))
            {
                occurrenceCount++;

                var isAfterStart = cursor >= windowStart;
                var isBeforeExplicitEnd = !recurrenceEnd.HasValue || cursor <= recurrenceEnd.Value;
                var isNotSkipped = !skippedDays.Contains(cursor);
                var isUntilDateValid = recurringBookingEndType != RecurringBookingEndType.UntilDate || isBeforeExplicitEnd;
                var isWithinOccurrenceLimit = !occurrenceLimit.HasValue || occurrenceCount <= occurrenceLimit.Value;

                if (isAfterStart && isUntilDateValid && isWithinOccurrenceLimit && isNotSkipped)
                {
                    days.Add(cursor);
                }

                if (occurrenceLimit.HasValue && occurrenceCount >= occurrenceLimit.Value)
                {
                    break;
                }
            }

            if (recurringBookingEndType == RecurringBookingEndType.UntilDate && recurrenceEnd.HasValue && cursor >= recurrenceEnd.Value)
            {
                break;
            }

            cursor = cursor.AddDays(1);
        }

        // "HasMoreRequiredBookingDays" means we still have recurrence work after the current planning window.
        var hasMoreRequiredBookingDays = HasAnyRequiredBookingDayOnOrAfter(
            recurringBooking,
            windowEndExclusive,
            recurrenceStart,
            recurrenceEnd,
            skippedDays,
            interval,
            recurringBookingEndType,
            occurrenceLimit);

        return (days.ToList(), hasMoreRequiredBookingDays);
    }
}
