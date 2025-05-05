namespace Api.Shared.Services.Models;

public enum PaymentStatus
{
    NoPaymentRequired,
    Pending,
    Paid,
    Unpaid,
    Expired
}

public static class PaymentStatusConstants
{
    public const string NoPaymentRequired = "NO_PAYMENT_REQUIRED";
    public const string Pending = "PENDING";
    public const string Paid = "PAID";
    public const string Unpaid = "UNPAID";
    public const string Expired = "EXPIRED";
}

public static class PaymentStatusExtensions
{
    public static PaymentStatus ToPaymentStatus(this string src) =>
        src switch
        {
            PaymentStatusConstants.NoPaymentRequired => PaymentStatus.NoPaymentRequired,
            PaymentStatusConstants.Pending => PaymentStatus.Pending,
            PaymentStatusConstants.Paid => PaymentStatus.Paid,
            PaymentStatusConstants.Unpaid => PaymentStatus.Unpaid,
            PaymentStatusConstants.Expired => PaymentStatus.Expired,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static PaymentStatus? ToNullablePaymentStatus(this string? src) =>
        string.IsNullOrWhiteSpace(src)
            ? null
            : src switch
            {
                PaymentStatusConstants.NoPaymentRequired => PaymentStatus.NoPaymentRequired,
                PaymentStatusConstants.Pending => PaymentStatus.Pending,
                PaymentStatusConstants.Paid => PaymentStatus.Paid,
                PaymentStatusConstants.Unpaid => PaymentStatus.Unpaid,
                PaymentStatusConstants.Expired => PaymentStatus.Expired,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToPaymentStatus(this PaymentStatus src) =>
        src switch
        {
            PaymentStatus.NoPaymentRequired => PaymentStatusConstants.NoPaymentRequired,
            PaymentStatus.Pending => PaymentStatusConstants.Pending,
            PaymentStatus.Paid => PaymentStatusConstants.Paid,
            PaymentStatus.Unpaid => PaymentStatusConstants.Unpaid,
            PaymentStatus.Expired => PaymentStatusConstants.Expired,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToNullablePaymentStatus(this PaymentStatus? src) =>
        src is null
            ? string.Empty
            : src switch
            {
                PaymentStatus.NoPaymentRequired => PaymentStatusConstants.NoPaymentRequired,
                PaymentStatus.Pending => PaymentStatusConstants.Pending,
                PaymentStatus.Paid => PaymentStatusConstants.Paid,
                PaymentStatus.Unpaid => PaymentStatusConstants.Unpaid,
                PaymentStatus.Expired => PaymentStatusConstants.Expired,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToPaymentStatusName(this PaymentStatus src) =>
        src switch
        {
            PaymentStatus.NoPaymentRequired => "No payment required",
            PaymentStatus.Pending => "Pending",
            PaymentStatus.Paid => "Paid",
            PaymentStatus.Unpaid => "Unpaid",
            PaymentStatus.Expired => "Expired",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToPaymentStatusName(this string src) =>
        src switch
        {
            PaymentStatusConstants.NoPaymentRequired => "No payment required",
            PaymentStatusConstants.Pending => "Pending",
            PaymentStatusConstants.Paid => "Paid",
            PaymentStatusConstants.Unpaid => "Unpaid",
            PaymentStatusConstants.Expired => "Expired",
            _ => throw new ArgumentOutOfRangeException()
        };
}
