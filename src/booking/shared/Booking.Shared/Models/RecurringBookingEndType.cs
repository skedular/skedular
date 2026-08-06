namespace Booking.Shared.Models;

public enum RecurringBookingEndType
{
    Never,
    UntilDate,
    AfterOccurrences,
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
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
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
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };

        public string ToRecurringBookingEndTypeName() =>
            src switch
            {
                RecurringBookingEndType.Never => "Never",
                RecurringBookingEndType.UntilDate => "Until Date",
                RecurringBookingEndType.AfterOccurrences => "After Occurrences",
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };
    }
}
