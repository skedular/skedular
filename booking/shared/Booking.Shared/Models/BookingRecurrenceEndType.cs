namespace Booking.Shared.Models;

public enum BookingRecurrenceEndType
{
    Never,
    UntilDate,
    AfterOccurrences
}

public static class BookingRecurrenceEndTypeConstants
{
    public const string Never = "NEVER";
    public const string UntilDate = "UNTIL_DATE";
    public const string AfterOccurrences = "AFTER_OCCURRENCES";
}

public static class BookingRecurrenceEndTypeExtensions
{
    extension(string src)
    {
        public BookingRecurrenceEndType ToBookingRecurrenceEndType() =>
            src switch
            {
                BookingRecurrenceEndTypeConstants.Never => BookingRecurrenceEndType.Never,
                BookingRecurrenceEndTypeConstants.UntilDate => BookingRecurrenceEndType.UntilDate,
                BookingRecurrenceEndTypeConstants.AfterOccurrences => BookingRecurrenceEndType.AfterOccurrences,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToBookingRecurrenceEndTypeName() =>
            src switch
            {
                BookingRecurrenceEndTypeConstants.Never => "Never",
                BookingRecurrenceEndTypeConstants.UntilDate => "Until Date",
                BookingRecurrenceEndTypeConstants.AfterOccurrences => "After Occurrences",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string? src)
    {
        public BookingRecurrenceEndType? ToNullableBookingRecurrenceEndType() =>
            string.IsNullOrWhiteSpace(src)
                ? null
                : src switch
                {
                    BookingRecurrenceEndTypeConstants.Never => BookingRecurrenceEndType.Never,
                    BookingRecurrenceEndTypeConstants.UntilDate => BookingRecurrenceEndType.UntilDate,
                    BookingRecurrenceEndTypeConstants.AfterOccurrences => BookingRecurrenceEndType.AfterOccurrences,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }

    extension(BookingRecurrenceEndType src)
    {
        public string ToBookingRecurrenceEndType() =>
            src switch
            {
                BookingRecurrenceEndType.Never => BookingRecurrenceEndTypeConstants.Never,
                BookingRecurrenceEndType.UntilDate => BookingRecurrenceEndTypeConstants.UntilDate,
                BookingRecurrenceEndType.AfterOccurrences => BookingRecurrenceEndTypeConstants.AfterOccurrences,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToBookingRecurrenceEndTypeName() =>
            src switch
            {
                BookingRecurrenceEndType.Never => "Never",
                BookingRecurrenceEndType.UntilDate => "Until Date",
                BookingRecurrenceEndType.AfterOccurrences => "After Occurrences",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(BookingRecurrenceEndType? src)
    {
        public string? ToNullableBookingRecurrenceEndType() =>
            src is null
                ? null
                : src switch
                {
                    BookingRecurrenceEndType.Never => BookingRecurrenceEndTypeConstants.Never,
                    BookingRecurrenceEndType.UntilDate => BookingRecurrenceEndTypeConstants.UntilDate,
                    BookingRecurrenceEndType.AfterOccurrences => BookingRecurrenceEndTypeConstants.AfterOccurrences,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }
}
