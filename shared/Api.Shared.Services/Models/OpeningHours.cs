namespace Api.Shared.Services.Models;

public record OpeningHoursDetails
{
    public static OpeningHoursDetails Default = new(false, true, null, null);

    public OpeningHoursDetails(bool closed, bool openAllDay, TimeOnly? from, TimeOnly? until)
    {
        if (closed && openAllDay)
        {
            throw new ArgumentException("openAllDay can't be set while closed is true.", nameof(openAllDay));
        }

        Closed = closed;
        OpenAllDay = openAllDay;

        if (Closed || OpenAllDay)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(from, nameof(from));
        ArgumentNullException.ThrowIfNull(until, nameof(until));

        if (from.Value >= until.Value)
        {
            throw new ArgumentOutOfRangeException(nameof(from), from, "from must be less than until");
        }

        if (!IsMultipleOf15(from.Value) || !IsMultipleOf15(until.Value))
        {
            throw new ArgumentException("Both from and until must be in 15-minute increments.");
        }

        From = from.Value;
        Until = until.Value;
    }

    public bool Closed { get; }
    public bool OpenAllDay { get; }
    public TimeOnly? From { get; }
    public TimeOnly? Until { get; }

    private static bool IsMultipleOf15(TimeOnly time) => time.Minute % 15 == 0;
}

public record WeekOpeningHours(
    OpeningHoursDetails Monday,
    OpeningHoursDetails Tuesday,
    OpeningHoursDetails Wednesday,
    OpeningHoursDetails Thursday,
    OpeningHoursDetails Friday,
    OpeningHoursDetails Saturday,
    OpeningHoursDetails Sunday);

public record OpeningHours(
    WeekOpeningHours WeekOpeningHours,
    ICollection<DateTimeOffset> ClosedDates,
    Dictionary<DateTimeOffset, OpeningHoursDetails> DatesWithVariedOpeningHours);
