namespace Api.Shared.Services.Models;

public enum PaymentStatus
{
    Template,
    Pending,
    Rejected,
    Confirmed,
    Expired,
    RecordNeverCreated,
    NoPaymentRequired
}

public static class PaymentStatusConstants
{
    public const string Template = "TEMPLATE";
    public const string Pending = "PENDING";
    public const string Rejected = "REJECTED";
    public const string Confirmed = "CONFIRMED";
    public const string Expired = "EXPIRED";
    public const string RecordNeverCreated = "RECORD_NEVER_CREATED";
    public const string NoPaymentRequired = "NO_PAYMENT_REQUIRED";
}

public static class PaymentStatusExtensions
{
    extension(string src)
    {
        public PaymentStatus ToPaymentStatus() =>
            src switch
            {
                PaymentStatusConstants.Template => PaymentStatus.Template,
                PaymentStatusConstants.Pending => PaymentStatus.Pending,
                PaymentStatusConstants.Rejected => PaymentStatus.Rejected,
                PaymentStatusConstants.Confirmed => PaymentStatus.Confirmed,
                PaymentStatusConstants.Expired => PaymentStatus.Expired,
                PaymentStatusConstants.RecordNeverCreated => PaymentStatus.RecordNeverCreated,
                PaymentStatusConstants.NoPaymentRequired => PaymentStatus.NoPaymentRequired,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToPaymentStatusName() =>
            src switch
            {
                PaymentStatusConstants.Template => "Template",
                PaymentStatusConstants.Pending => "Pending Payment",
                PaymentStatusConstants.Rejected => "Payment Rejected",
                PaymentStatusConstants.Confirmed => "Payment Confirmed",
                PaymentStatusConstants.Expired => "Payment Expired",
                PaymentStatusConstants.RecordNeverCreated => "Required Payment Record Never Created",
                PaymentStatusConstants.NoPaymentRequired => "No Payment Required",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(PaymentStatus src)
    {
        public string ToPaymentStatus() =>
            src switch
            {
                PaymentStatus.Template => PaymentStatusConstants.Template,
                PaymentStatus.Pending => PaymentStatusConstants.Pending,
                PaymentStatus.Rejected => PaymentStatusConstants.Rejected,
                PaymentStatus.Confirmed => PaymentStatusConstants.Confirmed,
                PaymentStatus.Expired => PaymentStatusConstants.Expired,
                PaymentStatus.RecordNeverCreated => PaymentStatusConstants.RecordNeverCreated,
                PaymentStatus.NoPaymentRequired => PaymentStatusConstants.NoPaymentRequired,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToPaymentStatusName() =>
            src switch
            {
                PaymentStatus.Template => "Template",
                PaymentStatus.Pending => "Pending Payment",
                PaymentStatus.Rejected => "Payment Rejected",
                PaymentStatus.Confirmed => "Payment Confirmed",
                PaymentStatus.Expired => "Payment Expired",
                PaymentStatus.RecordNeverCreated => "Required Payment Record Never Created",
                PaymentStatus.NoPaymentRequired => "No Payment Required",
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
