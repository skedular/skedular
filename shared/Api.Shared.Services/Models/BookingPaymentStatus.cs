namespace Api.Shared.Services.Models;

public enum BookingPaymentStatus
{
    Pending,
    Rejected,
    Confirmed,
    Expired,
    RecordNeverCreated
}

public static class BookingPaymentStatusConstants
{
    public const string Pending = "PENDING";
    public const string Rejected = "REJECTED";
    public const string Confirmed = "CONFIRMED";
    public const string Expired = "EXPIRED";
    public const string RecordNeverCreated = "RECORD_NEVER_CREATED";
}

public static class BookingStatusExtensions
{
    public static BookingPaymentStatus ToBookingPaymentStatus(this string src) =>
        src switch
        {
            BookingPaymentStatusConstants.Pending => BookingPaymentStatus.Pending,
            BookingPaymentStatusConstants.Rejected => BookingPaymentStatus.Rejected,
            BookingPaymentStatusConstants.Confirmed => BookingPaymentStatus.Confirmed,
            BookingPaymentStatusConstants.Expired => BookingPaymentStatus.Expired,
            BookingPaymentStatusConstants.RecordNeverCreated => BookingPaymentStatus.RecordNeverCreated,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static BookingPaymentStatus? ToNullableBookingPaymentStatus(this string? src) =>
        string.IsNullOrWhiteSpace(src)
            ? null
            : src switch
            {
                BookingPaymentStatusConstants.Pending => BookingPaymentStatus.Pending,
                BookingPaymentStatusConstants.Rejected => BookingPaymentStatus.Rejected,
                BookingPaymentStatusConstants.Confirmed => BookingPaymentStatus.Confirmed,
                BookingPaymentStatusConstants.Expired => BookingPaymentStatus.Expired,
                BookingPaymentStatusConstants.RecordNeverCreated => BookingPaymentStatus.RecordNeverCreated,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToBookingPaymentStatus(this BookingPaymentStatus src) =>
        src switch
        {
            BookingPaymentStatus.Pending => BookingPaymentStatusConstants.Pending,
            BookingPaymentStatus.Rejected => BookingPaymentStatusConstants.Rejected,
            BookingPaymentStatus.Confirmed => BookingPaymentStatusConstants.Confirmed,
            BookingPaymentStatus.Expired => BookingPaymentStatusConstants.Expired,
            BookingPaymentStatus.RecordNeverCreated => BookingPaymentStatusConstants.RecordNeverCreated,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToNullableBookingPaymentStatus(this BookingPaymentStatus? src) =>
        src is null
            ? string.Empty
            : src switch
            {
                BookingPaymentStatus.Pending => BookingPaymentStatusConstants.Pending,
                BookingPaymentStatus.Rejected => BookingPaymentStatusConstants.Rejected,
                BookingPaymentStatus.Confirmed => BookingPaymentStatusConstants.Confirmed,
                BookingPaymentStatus.Expired => BookingPaymentStatusConstants.Expired,
                BookingPaymentStatus.RecordNeverCreated => BookingPaymentStatusConstants.RecordNeverCreated,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToBookingPaymentStatusName(this BookingPaymentStatus src) =>
        src switch
        {
            BookingPaymentStatus.Pending => "Pending payment",
            BookingPaymentStatus.Rejected => "Payment rejected",
            BookingPaymentStatus.Confirmed => "Payment confirmed",
            BookingPaymentStatus.Expired => "Payment expired",
            BookingPaymentStatus.RecordNeverCreated => "Payment record never created",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToBookingPaymentStatusName(this string src) =>
        src switch
        {
            BookingPaymentStatusConstants.Pending => "Pending payment",
            BookingPaymentStatusConstants.Rejected => "Payment rejected",
            BookingPaymentStatusConstants.Confirmed => "Payment confirmed",
            BookingPaymentStatusConstants.Expired => "Payment expired",
            BookingPaymentStatusConstants.RecordNeverCreated => "Payment record never created",
            _ => throw new ArgumentOutOfRangeException()
        };
}
