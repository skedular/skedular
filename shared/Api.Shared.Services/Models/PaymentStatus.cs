namespace Api.Shared.Services.Models;

public enum PaymentStatus
{
    Pending,
    Rejected,
    Confirmed,
    Expired,
    RecordNeverCreated,
    NoPaymentRequired
}

public static class PaymentStatusConstants
{
    public const string Pending = "PENDING";
    public const string Rejected = "REJECTED";
    public const string Confirmed = "CONFIRMED";
    public const string Expired = "EXPIRED";
    public const string RecordNeverCreated = "RECORD_NEVER_CREATED";
    public const string NoPaymentRequired = "NO_PAYMENT_REQUIRED";
}

public static class PaymentStatusExtensions
{
    public static PaymentStatus ToPaymentStatus(this string src) =>
        src switch
        {
            PaymentStatusConstants.Pending => PaymentStatus.Pending,
            PaymentStatusConstants.Rejected => PaymentStatus.Rejected,
            PaymentStatusConstants.Confirmed => PaymentStatus.Confirmed,
            PaymentStatusConstants.Expired => PaymentStatus.Expired,
            PaymentStatusConstants.RecordNeverCreated => PaymentStatus.RecordNeverCreated,
            PaymentStatusConstants.NoPaymentRequired => PaymentStatus.NoPaymentRequired,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static PaymentStatus? ToNullablePaymentStatus(this string? src) =>
        string.IsNullOrWhiteSpace(src)
            ? null
            : src switch
            {
                PaymentStatusConstants.Pending => PaymentStatus.Pending,
                PaymentStatusConstants.Rejected => PaymentStatus.Rejected,
                PaymentStatusConstants.Confirmed => PaymentStatus.Confirmed,
                PaymentStatusConstants.Expired => PaymentStatus.Expired,
                PaymentStatusConstants.RecordNeverCreated => PaymentStatus.RecordNeverCreated,
                PaymentStatusConstants.NoPaymentRequired => PaymentStatus.NoPaymentRequired,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToPaymentStatus(this PaymentStatus src) =>
        src switch
        {
            PaymentStatus.Pending => PaymentStatusConstants.Pending,
            PaymentStatus.Rejected => PaymentStatusConstants.Rejected,
            PaymentStatus.Confirmed => PaymentStatusConstants.Confirmed,
            PaymentStatus.Expired => PaymentStatusConstants.Expired,
            PaymentStatus.RecordNeverCreated => PaymentStatusConstants.RecordNeverCreated,
            PaymentStatus.NoPaymentRequired => PaymentStatusConstants.NoPaymentRequired,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToNullablePaymentStatus(this PaymentStatus? src) =>
        src is null
            ? string.Empty
            : src switch
            {
                PaymentStatus.Pending => PaymentStatusConstants.Pending,
                PaymentStatus.Rejected => PaymentStatusConstants.Rejected,
                PaymentStatus.Confirmed => PaymentStatusConstants.Confirmed,
                PaymentStatus.Expired => PaymentStatusConstants.Expired,
                PaymentStatus.RecordNeverCreated => PaymentStatusConstants.RecordNeverCreated,
                PaymentStatus.NoPaymentRequired => PaymentStatusConstants.NoPaymentRequired,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToPaymentStatusName(this PaymentStatus src) =>
        src switch
        {
            PaymentStatus.Pending => "Pending payment",
            PaymentStatus.Rejected => "Payment rejected",
            PaymentStatus.Confirmed => "Payment confirmed",
            PaymentStatus.Expired => "Payment expired",
            PaymentStatus.RecordNeverCreated => "Payment record never created",
            PaymentStatus.NoPaymentRequired => "No payment required",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToPaymentStatusName(this string src) =>
        src switch
        {
            PaymentStatusConstants.Pending => "Pending payment",
            PaymentStatusConstants.Rejected => "Payment rejected",
            PaymentStatusConstants.Confirmed => "Payment confirmed",
            PaymentStatusConstants.Expired => "Payment expired",
            PaymentStatusConstants.RecordNeverCreated => "Payment record never created",
            PaymentStatusConstants.NoPaymentRequired => "No payment required",
            _ => throw new ArgumentOutOfRangeException()
        };
}
