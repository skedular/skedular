namespace Api.Shared.Services.Models;

public enum BookingStatus
{
    PaymentPending,
    PaymentRejected,
    PaymentConfirmed,
    PaymentExpired,
    PaymentRecordNeverCreated
}

public static class BookingStatusConstants
{
    public const string PaymentPending = "PAYMENT_PENDING";
    public const string PaymentRejected = "PAYMENT_REJECTED";
    public const string PaymentConfirmed = "PAYMENT_CONFIRMED";
    public const string PaymentExpired = "PAYMENT_EXPIRED";
    public const string PaymentRecordNeverCreated = "PAYMENT_RECORD_NEVER_CREATED";
}

public static class BookingStatusExtensions
{
    public static BookingStatus ToBookingStatus(this string src) =>
        src switch
        {
            BookingStatusConstants.PaymentPending => BookingStatus.PaymentPending,
            BookingStatusConstants.PaymentRejected => BookingStatus.PaymentRejected,
            BookingStatusConstants.PaymentConfirmed => BookingStatus.PaymentConfirmed,
            BookingStatusConstants.PaymentExpired => BookingStatus.PaymentExpired,
            BookingStatusConstants.PaymentRecordNeverCreated => BookingStatus.PaymentRecordNeverCreated,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static BookingStatus? ToNullableBookingStatus(this string? src) =>
        string.IsNullOrWhiteSpace(src)
            ? null
            : src switch
            {
                BookingStatusConstants.PaymentPending => BookingStatus.PaymentPending,
                BookingStatusConstants.PaymentRejected => BookingStatus.PaymentRejected,
                BookingStatusConstants.PaymentConfirmed => BookingStatus.PaymentConfirmed,
                BookingStatusConstants.PaymentExpired => BookingStatus.PaymentExpired,
                BookingStatusConstants.PaymentRecordNeverCreated => BookingStatus.PaymentRecordNeverCreated,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToBookingStatus(this BookingStatus src) =>
        src switch
        {
            BookingStatus.PaymentPending => BookingStatusConstants.PaymentPending,
            BookingStatus.PaymentRejected => BookingStatusConstants.PaymentRejected,
            BookingStatus.PaymentConfirmed => BookingStatusConstants.PaymentConfirmed,
            BookingStatus.PaymentExpired => BookingStatusConstants.PaymentExpired,
            BookingStatus.PaymentRecordNeverCreated => BookingStatusConstants.PaymentRecordNeverCreated,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToNullableBookingStatus(this BookingStatus? src) =>
        src is null
            ? string.Empty
            : src switch
            {
                BookingStatus.PaymentPending => BookingStatusConstants.PaymentPending,
                BookingStatus.PaymentRejected => BookingStatusConstants.PaymentRejected,
                BookingStatus.PaymentConfirmed => BookingStatusConstants.PaymentConfirmed,
                BookingStatus.PaymentExpired => BookingStatusConstants.PaymentExpired,
                BookingStatus.PaymentRecordNeverCreated => BookingStatusConstants.PaymentRecordNeverCreated,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToBookingStatusName(this BookingStatus src) =>
        src switch
        {
            BookingStatus.PaymentPending => "Pending payment",
            BookingStatus.PaymentRejected => "Payment rejected",
            BookingStatus.PaymentConfirmed => "Payment confirmed",
            BookingStatus.PaymentExpired => "Payment expired",
            BookingStatus.PaymentRecordNeverCreated => "Payment record never created",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToBookingStatusName(this string src) =>
        src switch
        {
            BookingStatusConstants.PaymentPending => "Pending payment",
            BookingStatusConstants.PaymentRejected => "Payment rejected",
            BookingStatusConstants.PaymentConfirmed => "Payment confirmed",
            BookingStatusConstants.PaymentExpired => "Payment expired",
            BookingStatusConstants.PaymentRecordNeverCreated => "Payment record never created",
            _ => throw new ArgumentOutOfRangeException()
        };
}
