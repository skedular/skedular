namespace Api.Shared.Services.Models;

public static class DayOfWeekConstants
{
    public const string Monday = "MON";
    public const string Tuesday = "TUE";
    public const string Wednesday = "WED";
    public const string Thursday = "THU";
    public const string Friday = "FRI";
    public const string Saturday = "SAT";
    public const string Sunday = "SUN";
}

public static class DayOfWeekExtensions
{
    extension(string src)
    {
        public DayOfWeek ToDayOfWeek() =>
            src switch
            {
                DayOfWeekConstants.Monday => DayOfWeek.Monday,
                DayOfWeekConstants.Tuesday => DayOfWeek.Tuesday,
                DayOfWeekConstants.Wednesday => DayOfWeek.Wednesday,
                DayOfWeekConstants.Thursday => DayOfWeek.Thursday,
                DayOfWeekConstants.Friday => DayOfWeek.Friday,
                DayOfWeekConstants.Saturday => DayOfWeek.Saturday,
                DayOfWeekConstants.Sunday => DayOfWeek.Sunday,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
            };
    }

    extension(DayOfWeek src)
    {
        public string ToDayOfWeek() =>
            src switch
            {
                DayOfWeek.Monday => DayOfWeekConstants.Monday,
                DayOfWeek.Tuesday => DayOfWeekConstants.Tuesday,
                DayOfWeek.Wednesday => DayOfWeekConstants.Wednesday,
                DayOfWeek.Thursday => DayOfWeekConstants.Thursday,
                DayOfWeek.Friday => DayOfWeekConstants.Friday,
                DayOfWeek.Saturday => DayOfWeekConstants.Saturday,
                DayOfWeek.Sunday => DayOfWeekConstants.Sunday,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
            };

        public string ToDayOfWeekName() =>
            src switch
            {
                DayOfWeek.Monday => "Monday",
                DayOfWeek.Tuesday => "Tuesday",
                DayOfWeek.Wednesday => "Wednesday",
                DayOfWeek.Thursday => "Thursday",
                DayOfWeek.Friday => "Friday",
                DayOfWeek.Saturday => "Saturday",
                DayOfWeek.Sunday => "Sunday",
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
            };
    }
}
