namespace Api.Shared.Services.Models;

public enum BookingStatus
{
    Pending,
    Rejected,
    Confirmed,
    PaymentExpired,
    PaymentRecordNeverCreated
}

public static class BookingStatusConstants
{
    public const string Pending = "PENDING";
    public const string Rejected = "REJECTED";
    public const string Confirmed = "CONFIRMED";
    public const string PaymentExpired = "PAYMENT_EXPIRED";
    public const string PaymentRecordNeverCreated = "PAYMENT_RECORD_NEVER_CREATED";
}

public static class BookingStatusExtensions
{
    public static BookingStatus ToBookingStatus(this string src) =>
        src switch
        {
            BookingStatusConstants.Pending => BookingStatus.Pending,
            BookingStatusConstants.Rejected => BookingStatus.Rejected,
            BookingStatusConstants.Confirmed => BookingStatus.Confirmed,
            BookingStatusConstants.PaymentExpired => BookingStatus.PaymentExpired,
            BookingStatusConstants.PaymentRecordNeverCreated => BookingStatus.PaymentRecordNeverCreated,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static BookingStatus? ToNullableBookingStatus(this string? src) =>
        string.IsNullOrWhiteSpace(src)
            ? null
            : src switch
            {
                BookingStatusConstants.Pending => BookingStatus.Pending,
                BookingStatusConstants.Rejected => BookingStatus.Rejected,
                BookingStatusConstants.Confirmed => BookingStatus.Confirmed,
                BookingStatusConstants.PaymentExpired => BookingStatus.PaymentExpired,
                BookingStatusConstants.PaymentRecordNeverCreated => BookingStatus.PaymentRecordNeverCreated,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToBookingStatus(this BookingStatus src) =>
        src switch
        {
            BookingStatus.Pending => BookingStatusConstants.Pending,
            BookingStatus.Rejected => BookingStatusConstants.Rejected,
            BookingStatus.Confirmed => BookingStatusConstants.Confirmed,
            BookingStatus.PaymentExpired => BookingStatusConstants.PaymentExpired,
            BookingStatus.PaymentRecordNeverCreated => BookingStatusConstants.PaymentRecordNeverCreated,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToNullableBookingStatus(this BookingStatus? src) =>
        src is null
            ? string.Empty
            : src switch
            {
                BookingStatus.Pending => BookingStatusConstants.Pending,
                BookingStatus.Rejected => BookingStatusConstants.Rejected,
                BookingStatus.Confirmed => BookingStatusConstants.Confirmed,
                BookingStatus.PaymentExpired => BookingStatusConstants.PaymentExpired,
                BookingStatus.PaymentRecordNeverCreated => BookingStatusConstants.PaymentRecordNeverCreated,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToBookingStatusName(this BookingStatus src) =>
        src switch
        {
            BookingStatus.Pending => "Pending",
            BookingStatus.Rejected => "Rejected",
            BookingStatus.Confirmed => "Confirmed",
            BookingStatus.PaymentExpired => "Payment expired",
            BookingStatus.PaymentRecordNeverCreated => "Payment record never created",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToBookingStatusName(this string src) =>
        src switch
        {
            BookingStatusConstants.Pending => "Pending",
            BookingStatusConstants.Rejected => "Rejected",
            BookingStatusConstants.Confirmed => "Confirmed",
            BookingStatusConstants.PaymentExpired => "Payment expired",
            BookingStatusConstants.PaymentRecordNeverCreated => "Payment record never created",
            _ => throw new ArgumentOutOfRangeException()
        };
}
