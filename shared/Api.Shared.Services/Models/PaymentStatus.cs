namespace Api.Shared.Services.Models;

public enum PaymentStatus
{
    NoPaymentRequired,
    Paid,
    Unpaid
}

public static class PaymentStatusConstants
{
    public const string NoPaymentRequired = "NO_PAYMENT_REQUIRED";
    public const string Paid = "PAID";
    public const string Unpaid = "UNPAID";
}

public static class PaymentStatusExtensions
{
    public static PaymentStatus ToPaymentStatus(this string src) =>
        src switch
        {
            PaymentStatusConstants.NoPaymentRequired => PaymentStatus.NoPaymentRequired,
            PaymentStatusConstants.Paid => PaymentStatus.Paid,
            PaymentStatusConstants.Unpaid => PaymentStatus.Unpaid,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static PaymentStatus? ToNullablePaymentStatus(this string? src) =>
        string.IsNullOrWhiteSpace(src)
            ? null
            : src switch
            {
                PaymentStatusConstants.NoPaymentRequired => PaymentStatus.NoPaymentRequired,
                PaymentStatusConstants.Paid => PaymentStatus.Paid,
                PaymentStatusConstants.Unpaid => PaymentStatus.Unpaid,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToPaymentStatus(this PaymentStatus src) =>
        src switch
        {
            PaymentStatus.NoPaymentRequired => PaymentStatusConstants.NoPaymentRequired,
            PaymentStatus.Paid => PaymentStatusConstants.Paid,
            PaymentStatus.Unpaid => PaymentStatusConstants.Unpaid,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToNullablePaymentStatus(this PaymentStatus? src) =>
        src is null
            ? string.Empty
            : src switch
            {
                PaymentStatus.NoPaymentRequired => PaymentStatusConstants.NoPaymentRequired,
                PaymentStatus.Paid => PaymentStatusConstants.Paid,
                PaymentStatus.Unpaid => PaymentStatusConstants.Unpaid,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToPaymentStatusName(this PaymentStatus src) =>
        src switch
        {
            PaymentStatus.NoPaymentRequired => "No payment required",
            PaymentStatus.Paid => "Paid",
            PaymentStatus.Unpaid => "Unpaid",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToPaymentStatusName(this string src) =>
        src switch
        {
            PaymentStatusConstants.NoPaymentRequired => "No payment required",
            PaymentStatusConstants.Paid => "Paid",
            PaymentStatusConstants.Unpaid => "Unpaid",
            _ => throw new ArgumentOutOfRangeException()
        };
}
