namespace Api.Shared.Services.Models;

public enum Weekday
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    Sunday
}

public static class WeekdayConstants
{
    public const string Monday = "MON";
    public const string Tuesday = "TUE";
    public const string Wednesday = "WED";
    public const string Thursday = "THU";
    public const string Friday = "FRI";
    public const string Saturday = "SAT";
    public const string Sunday = "SUN";
}

public static class WeekdayExtensions
{
    extension(string src)
    {
        public Weekday ToWeekday() =>
            src switch
            {
                WeekdayConstants.Monday => Weekday.Monday,
                WeekdayConstants.Tuesday => Weekday.Tuesday,
                WeekdayConstants.Wednesday => Weekday.Wednesday,
                WeekdayConstants.Thursday => Weekday.Thursday,
                WeekdayConstants.Friday => Weekday.Friday,
                WeekdayConstants.Saturday => Weekday.Saturday,
                WeekdayConstants.Sunday => Weekday.Sunday,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToWeekdayName() =>
            src switch
            {
                WeekdayConstants.Monday => "Monday",
                WeekdayConstants.Tuesday => "Tuesday",
                WeekdayConstants.Wednesday => "Wednesday",
                WeekdayConstants.Thursday => "Thursday",
                WeekdayConstants.Friday => "Friday",
                WeekdayConstants.Saturday => "Saturday",
                WeekdayConstants.Sunday => "Sunday",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string? src)
    {
        public Weekday? ToNullableWeekday() =>
            string.IsNullOrWhiteSpace(src)
                ? null
                : src switch
                {
                    WeekdayConstants.Monday => Weekday.Monday,
                    WeekdayConstants.Tuesday => Weekday.Tuesday,
                    WeekdayConstants.Wednesday => Weekday.Wednesday,
                    WeekdayConstants.Thursday => Weekday.Thursday,
                    WeekdayConstants.Friday => Weekday.Friday,
                    WeekdayConstants.Saturday => Weekday.Saturday,
                    WeekdayConstants.Sunday => Weekday.Sunday,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }

    extension(Weekday src)
    {
        public string ToWeekday() =>
            src switch
            {
                Weekday.Monday => WeekdayConstants.Monday,
                Weekday.Tuesday => WeekdayConstants.Tuesday,
                Weekday.Wednesday => WeekdayConstants.Wednesday,
                Weekday.Thursday => WeekdayConstants.Thursday,
                Weekday.Friday => WeekdayConstants.Friday,
                Weekday.Saturday => WeekdayConstants.Saturday,
                Weekday.Sunday => WeekdayConstants.Sunday,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToWeekdayName() =>
            src switch
            {
                Weekday.Monday => "Monday",
                Weekday.Tuesday => "Tuesday",
                Weekday.Wednesday => "Wednesday",
                Weekday.Thursday => "Thursday",
                Weekday.Friday => "Friday",
                Weekday.Saturday => "Saturday",
                Weekday.Sunday => "Sunday",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(Weekday? src)
    {
        public string? ToNullableWeekday() =>
            src is null
                ? null
                : src switch
                {
                    Weekday.Monday => WeekdayConstants.Monday,
                    Weekday.Tuesday => WeekdayConstants.Tuesday,
                    Weekday.Wednesday => WeekdayConstants.Wednesday,
                    Weekday.Thursday => WeekdayConstants.Thursday,
                    Weekday.Friday => WeekdayConstants.Friday,
                    Weekday.Saturday => WeekdayConstants.Saturday,
                    Weekday.Sunday => WeekdayConstants.Sunday,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }
}
