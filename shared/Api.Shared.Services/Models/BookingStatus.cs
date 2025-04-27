namespace Api.Shared.Services.Models;

public enum BookingStatus
{
    Pending,
    Rejected,
    Confirmed
}

public static class BookingStatusConstants
{
    public const string Pending = "PENDING";
    public const string Rejected = "REJECTED";
    public const string Confirmed = "CONFIRMED";
}

public static class BookingStatusExtensions
{
    public static BookingStatus ToBookingStatus(this string src) =>
        src switch
        {
            BookingStatusConstants.Pending => BookingStatus.Pending,
            BookingStatusConstants.Rejected => BookingStatus.Rejected,
            BookingStatusConstants.Confirmed => BookingStatus.Confirmed,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToBookingStatus(this BookingStatus src) =>
        src switch
        {
            BookingStatus.Pending => BookingStatusConstants.Pending,
            BookingStatus.Rejected => BookingStatusConstants.Rejected,
            BookingStatus.Confirmed => BookingStatusConstants.Confirmed,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToBookingStatusName(this BookingStatus src) =>
        src switch
        {
            BookingStatus.Pending => "Pending",
            BookingStatus.Rejected => "Rejected",
            BookingStatus.Confirmed => "Confirmed",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToBookingStatusName(this string src) =>
        src switch
        {
            BookingStatusConstants.Pending => "Pending",
            BookingStatusConstants.Rejected => "Rejected",
            BookingStatusConstants.Confirmed => "Confirmed",
            _ => throw new ArgumentOutOfRangeException()
        };
}
