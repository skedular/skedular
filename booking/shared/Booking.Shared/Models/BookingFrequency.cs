namespace Booking.Shared.Models;

public enum BookingFrequency
{
    Daily,
    Weekly,
    Monthly
}

public static class BookingFrequencyConstants
{
    public const string Daily = "DAILY";
    public const string Weekly = "WEEKLY";
    public const string Monthly = "MONTHLY";
}

public static class BookingFrequencyExtensions
{
    extension(string src)
    {
        public BookingFrequency ToBookingFrequency() =>
            src switch
            {
                BookingFrequencyConstants.Daily => BookingFrequency.Daily,
                BookingFrequencyConstants.Weekly => BookingFrequency.Weekly,
                BookingFrequencyConstants.Monthly => BookingFrequency.Monthly,
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(BookingFrequency src)
    {
        public string ToBookingFrequency() =>
            src switch
            {
                BookingFrequency.Daily => BookingFrequencyConstants.Daily,
                BookingFrequency.Weekly => BookingFrequencyConstants.Weekly,
                BookingFrequency.Monthly => BookingFrequencyConstants.Monthly,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToBookingFrequencyName() =>
            src switch
            {
                BookingFrequency.Daily => "Daily",
                BookingFrequency.Weekly => "Weekly",
                BookingFrequency.Monthly => "Monthly",
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
