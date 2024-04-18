namespace Enterprise.Shared.Time;

public static class DateTimeOffsetExtensions
{
    public static DateTimeOffset ToDateTimeOffset(this DateTime value) => new(value.Ticks, TimeSpan.Zero);

    public static DateTime ToDateTime(this DateTimeOffset value) => value.DateTime;

    public static DateTimeOffset StartOfDay(this DateTimeOffset value) => value.ToDate();

    public static DateTimeOffset EndOfDay(this DateTimeOffset value) => value.StartOfDay().AddDays(1).AddTicks(-1);

    public static DateTimeOffset EndOfYesterday(this DateTimeOffset value) => value.StartOfDay().AddTicks(-1);

    public static DateTimeOffset Tomorrow(this DateTimeOffset value) => value.AddDays(1);

    public static DateTimeOffset ToDate(this DateTimeOffset dateTimeOffset) =>
        new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, 0, 0, 0, dateTimeOffset.Offset);

    public static DateTimeOffset StartOfDay(this DateTimeOffset dateTimeOffset, TimeZoneInfo timeZoneInfo)
    {
        var converted = TimeZoneInfo.ConvertTime(dateTimeOffset, timeZoneInfo);
        return new DateTimeOffset(converted.Year, converted.Month, converted.Day, 0, 0, 0, converted.Offset);
    }

    public static DateTimeOffset StartOfWeek(
        this DateTimeOffset dateTimeOffset,
        DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        var diff = (7 + (dateTimeOffset.DayOfWeek - startOfWeek)) % 7;
        return new DateTimeOffset(dateTimeOffset.AddDays(-1 * diff).Date, dateTimeOffset.Offset);
    }

    public static string ToShortDateWithoutYear(this DateTimeOffset value) => value.ToString("dddd, dd'th' MMMM");

    public static TimeZoneInfo ToTimezoneInfo(this string? timezone)
    {
        try
        {
            return string.IsNullOrWhiteSpace(timezone)
                ? TimeZoneInfo.Utc
                : TimeZoneInfo.FindSystemTimeZoneById(timezone);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.Utc;
        }
    }

    public static bool IsMatchingHour(
        this DateTimeOffset value,
        string? timezone,
        int hour) =>
        value.IsMatchingHour(timezone.ToTimezoneInfo(), hour);

    public static bool IsMatchingHour(
        this DateTimeOffset value,
        TimeZoneInfo timezoneInfo,
        int hour) =>
        TimeZoneInfo.ConvertTime(value, timezoneInfo).Hour == hour;
}
