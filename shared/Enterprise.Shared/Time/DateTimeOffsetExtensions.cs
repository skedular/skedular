namespace Enterprise.Shared.Time;

public static class DateTimeOffsetExtensions
{
    extension(DateTime value)
    {
        public DateTimeOffset ToDateTimeOffset() => new(value.Ticks, TimeSpan.Zero);
    }

    extension(DateTimeOffset value)
    {
        public DateTimeOffset TrimAllAfterSeconds() => new(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Offset);
        public DateTime ToDateTime() => value.DateTime;
        public DateTimeOffset StartOfDay() => value.ToDate();
        public DateTimeOffset EndOfDay() => value.StartOfDay().AddDays(1);
        public DateTimeOffset EndOfYesterday() => value.StartOfDay().AddTicks(-1);
        public DateTimeOffset ToDate() => value.ToDate(value.Offset);
        public DateTimeOffset ToDate(TimeSpan offset) => new(value.Year, value.Month, value.Day, 0, 0, 0, offset);

        public DateTimeOffset StartOfWeek(DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            var diff = (7 + (value.DayOfWeek - startOfWeek)) % 7;
            return new DateTimeOffset(value.AddDays(-1 * diff).Date, value.Offset);
        }

        public string ToShortDateWithoutYear() => value.ToString("dd MMMM");
        public string ToShortDate() => value.ToString("dd MMMM yyyy");
        public string ToShortTime() => value.ToString("HH:mm");
        public bool IsMatchingHour(string? timezone, int hour) => value.IsMatchingHour(timezone.ToTimezoneInfo(), hour);
        public bool IsMatchingHour(TimeZoneInfo timezoneInfo, int hour) => TimeZoneInfo.ConvertTime(value, timezoneInfo).Hour == hour;
    }

    extension(string? timezone)
    {
        public TimeZoneInfo ToTimezoneInfo()
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
    }

    extension(DateOnly date)
    {
        public DateTimeOffset ToDateTimeOffset(TimeSpan timeSpan) => new(
            date.Year,
            date.Month,
            date.Day,
            timeSpan.Hours,
            timeSpan.Minutes,
            timeSpan.Seconds,
            timeSpan.Milliseconds,
            timeSpan.Microseconds,
            TimeSpan.Zero);
    }
}
