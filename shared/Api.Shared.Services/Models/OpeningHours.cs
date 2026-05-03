namespace Api.Shared.Services.Models;

public record OpeningHoursDetails
{
    public static readonly int BookingSlotSizeInMinutes = 15;
    public static readonly OpeningHoursDetails Default = new(false, true);

    public OpeningHoursDetails(bool closed, bool openAllDay, TimeOnly? from = null, TimeOnly? until = null)
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

        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(until);

        if (from.Value >= until.Value)
        {
            throw new ArgumentOutOfRangeException(nameof(from), from, "from must be less than until");
        }

        if (!IsMultipleOfOpeningHoursSlotSize(from.Value) || !IsMultipleOfOpeningHoursSlotSize(until.Value))
        {
            throw new ArgumentException($"Both from and until must be in {BookingSlotSizeInMinutes}-minute increments.");
        }

        From = from.Value;
        Until = until.Value;
    }

    public bool Closed { get; }
    public bool OpenAllDay { get; }
    public TimeOnly? From { get; }
    public TimeOnly? Until { get; }

    private static bool IsMultipleOfOpeningHoursSlotSize(TimeOnly time) => time.Minute % BookingSlotSizeInMinutes == 0;
}

public record WeekOpeningHours(
    OpeningHoursDetails Monday,
    OpeningHoursDetails Tuesday,
    OpeningHoursDetails Wednesday,
    OpeningHoursDetails Thursday,
    OpeningHoursDetails Friday,
    OpeningHoursDetails Saturday,
    OpeningHoursDetails Sunday)
{
    public static readonly WeekOpeningHours Default = new(
        OpeningHoursDetails.Default,
        OpeningHoursDetails.Default,
        OpeningHoursDetails.Default,
        OpeningHoursDetails.Default,
        OpeningHoursDetails.Default,
        OpeningHoursDetails.Default,
        OpeningHoursDetails.Default);
}

public record OpeningHours(
    WeekOpeningHours WeekOpeningHours,
    IReadOnlyList<DateTimeOffset> ClosedDates,
    Dictionary<DateTimeOffset, OpeningHoursDetails> DatesWithVariedOpeningHours)
{
    public static readonly OpeningHours Default = new(WeekOpeningHours.Default, [], []);

    public virtual bool Equals(OpeningHours? other)
    {
        if (other is null)
        {
            return false;
        }

        return other.WeekOpeningHours.Monday.Equals(WeekOpeningHours.Monday) &&
               other.WeekOpeningHours.Tuesday.Equals(WeekOpeningHours.Tuesday) &&
               other.WeekOpeningHours.Wednesday.Equals(WeekOpeningHours.Wednesday) &&
               other.WeekOpeningHours.Thursday.Equals(WeekOpeningHours.Thursday) &&
               other.WeekOpeningHours.Friday.Equals(WeekOpeningHours.Friday) &&
               other.WeekOpeningHours.Saturday.Equals(WeekOpeningHours.Saturday) &&
               other.WeekOpeningHours.Sunday.Equals(WeekOpeningHours.Sunday) &&
               other.ClosedDates.SequenceEqual(other.ClosedDates) &&
               other.DatesWithVariedOpeningHours.SequenceEqual(other.DatesWithVariedOpeningHours);
    }

    public override int GetHashCode() => HashCode.Combine(WeekOpeningHours, ClosedDates, DatesWithVariedOpeningHours);
}

public static class OpeningHoursExtensions
{
    extension(OpeningHours? openingHours1)
    {
        public bool IsEqual(OpeningHours? openingHours2)
        {
            if (openingHours1 is null && openingHours2 is null)
            {
                return true;
            }

            if (openingHours1 is not null && openingHours2 is not null)
            {
                return openingHours1.Equals(openingHours2);
            }

            return true;
        }
    }
}
