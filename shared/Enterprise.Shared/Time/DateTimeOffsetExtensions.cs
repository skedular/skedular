namespace Enterprise.Shared.Time;

public static class DateTimeOffsetExtensions
{
    public static DateTimeOffset TrimAllAfterSeconds(this DateTimeOffset value) =>
        new(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Offset);

    public static DateTimeOffset ToDateTimeOffset(this DateTime value) => new(value.Ticks, TimeSpan.Zero);

    public static DateTime ToDateTime(this DateTimeOffset value) => value.DateTime;

    public static DateTimeOffset StartOfDay(this DateTimeOffset value) => value.ToDate();

    public static DateTimeOffset EndOfDay(this DateTimeOffset value) => value.StartOfDay().AddDays(1);

    public static DateTimeOffset EndOfYesterday(this DateTimeOffset value) => value.StartOfDay().AddTicks(-1);

    public static DateTimeOffset ToDate(this DateTimeOffset dateTimeOffset) => dateTimeOffset.ToDate(dateTimeOffset.Offset);

    public static DateTimeOffset ToDate(this DateTimeOffset dateTimeOffset, TimeSpan offset) =>
        new(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, 0, 0, 0, offset);

    public static DateTimeOffset StartOfWeek(this DateTimeOffset dateTimeOffset, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        var diff = (7 + (dateTimeOffset.DayOfWeek - startOfWeek)) % 7;
        return new DateTimeOffset(dateTimeOffset.AddDays(-1 * diff).Date, dateTimeOffset.Offset);
    }

    public static string ToShortDateWithoutYear(this DateTimeOffset value) => value.ToString("dd MMMM");

    public static TimeZoneInfo ToTimezoneInfo(this string? timezone)
    {
        try
        {
            return string.IsNullOrWhiteSpace(timezone) ? TimeZoneInfo.Utc : TimeZoneInfo.FindSystemTimeZoneById(timezone);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.Utc;
        }
    }

    public static bool IsMatchingHour(this DateTimeOffset value, string? timezone, int hour) =>
        value.IsMatchingHour(timezone.ToTimezoneInfo(), hour);

    public static bool IsMatchingHour(this DateTimeOffset value, TimeZoneInfo timezoneInfo, int hour) =>
        TimeZoneInfo.ConvertTime(value, timezoneInfo).Hour == hour;
}
