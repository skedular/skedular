namespace Booking.Shared.Models;

public enum RecurringBookingEndType
{
    Never,
    UntilDate,
    AfterOccurrences
}

public static class RecurringBookingEndTypeConstants
{
    public const string Never = "NEVER";
    public const string UntilDate = "UNTIL_DATE";
    public const string AfterOccurrences = "AFTER_OCCURRENCES";
}

public static class RecurringBookingEndTypeExtensions
{
    extension(string src)
    {
        public RecurringBookingEndType ToRecurringBookingEndType() =>
            src switch
            {
                RecurringBookingEndTypeConstants.Never => RecurringBookingEndType.Never,
                RecurringBookingEndTypeConstants.UntilDate => RecurringBookingEndType.UntilDate,
                RecurringBookingEndTypeConstants.AfterOccurrences => RecurringBookingEndType.AfterOccurrences,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToRecurringBookingEndTypeName() =>
            src switch
            {
                RecurringBookingEndTypeConstants.Never => "Never",
                RecurringBookingEndTypeConstants.UntilDate => "Until Date",
                RecurringBookingEndTypeConstants.AfterOccurrences => "After Occurrences",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string? src)
    {
        public RecurringBookingEndType? ToNullableRecurringBookingEndType() =>
            string.IsNullOrWhiteSpace(src)
                ? null
                : src switch
                {
                    RecurringBookingEndTypeConstants.Never => RecurringBookingEndType.Never,
                    RecurringBookingEndTypeConstants.UntilDate => RecurringBookingEndType.UntilDate,
                    RecurringBookingEndTypeConstants.AfterOccurrences => RecurringBookingEndType.AfterOccurrences,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }

    extension(RecurringBookingEndType src)
    {
        public string ToRecurringBookingEndType() =>
            src switch
            {
                RecurringBookingEndType.Never => RecurringBookingEndTypeConstants.Never,
                RecurringBookingEndType.UntilDate => RecurringBookingEndTypeConstants.UntilDate,
                RecurringBookingEndType.AfterOccurrences => RecurringBookingEndTypeConstants.AfterOccurrences,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToRecurringBookingEndTypeName() =>
            src switch
            {
                RecurringBookingEndType.Never => "Never",
                RecurringBookingEndType.UntilDate => "Until Date",
                RecurringBookingEndType.AfterOccurrences => "After Occurrences",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(RecurringBookingEndType? src)
    {
        public string? ToNullableRecurringBookingEndType() =>
            src is null
                ? null
                : src switch
                {
                    RecurringBookingEndType.Never => RecurringBookingEndTypeConstants.Never,
                    RecurringBookingEndType.UntilDate => RecurringBookingEndTypeConstants.UntilDate,
                    RecurringBookingEndType.AfterOccurrences => RecurringBookingEndTypeConstants.AfterOccurrences,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }
}
