namespace Api.Shared.Services.Models;

public class OpeningHour
{
    public OpeningHour(TimeOnly from, TimeOnly until)
    {
        if (from >= until)
        {
            throw new ArgumentOutOfRangeException(nameof(from), from, "from must be less than until");
        }

        if (!IsMultipleOf15(from) || !IsMultipleOf15(until))
        {
            throw new ArgumentException("Both from and until must be in 15-minute increments.");
        }

        From = from;
        Until = until;
    }

    public TimeOnly From { get; }
    public TimeOnly Until { get; }

    private static bool IsMultipleOf15(TimeOnly time) => time.Minute % 15 == 0;
}

public class OpeningHours(
    OpeningHour Monday,
    OpeningHour Tuesday,
    OpeningHour Wednesday,
    OpeningHour Thursday,
    OpeningHour Friday,
    OpeningHour Saturday,
    OpeningHour Sunday,
    ICollection<DateTimeOffset> ClosedDates,
    Dictionary<DateTimeOffset, OpeningHours> DatesWithVariedOpeningHours);
