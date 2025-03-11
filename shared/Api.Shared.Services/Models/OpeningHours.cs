namespace Api.Shared.Services.Models;

public record OpeningHoursDetails
{
    public OpeningHoursDetails(bool isClosed, bool open24, TimeOnly? from, TimeOnly? until)
    {
        if (isClosed && open24)
        {
            throw new ArgumentException("open24 can't be set while isClosed is true.", nameof(open24));
        }

        IsClosed = isClosed;
        Open24 = open24;

        if (IsClosed || Open24)
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

    public bool IsClosed { get; }
    public bool Open24 { get; }
    public TimeOnly? From { get; }
    public TimeOnly? Until { get; }

    private static bool IsMultipleOf15(TimeOnly time) => time.Minute % 15 == 0;
}

public record OpeningHours(
    OpeningHoursDetails Monday,
    OpeningHoursDetails Tuesday,
    OpeningHoursDetails Wednesday,
    OpeningHoursDetails Thursday,
    OpeningHoursDetails Friday,
    OpeningHoursDetails Saturday,
    OpeningHoursDetails Sunday,
    ICollection<DateTimeOffset> ClosedDates,
    Dictionary<DateTimeOffset, OpeningHoursDetails> DatesWithVariedOpeningHours);

