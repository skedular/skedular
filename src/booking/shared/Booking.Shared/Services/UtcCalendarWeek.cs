namespace Booking.Shared.Services;

public static class UtcCalendarWeek
{
    public static DateTimeOffset Start(DateTimeOffset value)
    {
        var date = value.UtcDateTime.Date;
        return new DateTimeOffset(date.AddDays(-(((int)date.DayOfWeek + 6) % 7)), TimeSpan.Zero);
    }

    public static bool IsComplete(DateTimeOffset weekStart, DateTimeOffset periodStart, DateTimeOffset periodEnd) =>
        weekStart >= Start(periodStart) && weekStart.AddDays(7) <= periodEnd.ToUniversalTime() &&
        periodStart.ToUniversalTime() <= weekStart;
}
