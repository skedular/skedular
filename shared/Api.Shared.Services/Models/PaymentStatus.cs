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

    public static string ToPaymentStatusName(this PaymentStatus src) =>
        src switch
        {
            PaymentStatus.Pending => "Pending Payment",
            PaymentStatus.Rejected => "Payment Rejected",
            PaymentStatus.Confirmed => "Payment Confirmed",
            PaymentStatus.Expired => "Payment Expired",
            PaymentStatus.RecordNeverCreated => "Required Payment Record Never Created",
            PaymentStatus.NoPaymentRequired => "No Payment Required",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToPaymentStatusName(this string src) =>
        src switch
        {
            PaymentStatusConstants.Pending => "Pending Payment",
            PaymentStatusConstants.Rejected => "Payment Rejected",
            PaymentStatusConstants.Confirmed => "Payment Confirmed",
            PaymentStatusConstants.Expired => "Payment Expired",
            PaymentStatusConstants.RecordNeverCreated => "Required Payment Record Never Created",
            PaymentStatusConstants.NoPaymentRequired => "No Payment Required",
            _ => throw new ArgumentOutOfRangeException()
        };
}
