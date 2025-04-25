namespace Api.Shared.Services.Models;

public enum StripePaymentMethodStatus
{
    Pending,
    Failed,
    Confirmed
}

public static class StripePaymentMethodStatusConstants
{
    public const string Pending = "PENDING";
    public const string Failed = "FAILED";
    public const string Confirmed = "CONFIRMED";
}

public static class StripePaymentMethodStatusExtensions
{
    public static StripePaymentMethodStatus ToStripePaymentMethodStatus(this string src) =>
        src switch
        {
            StripePaymentMethodStatusConstants.Pending => StripePaymentMethodStatus.Pending,
            StripePaymentMethodStatusConstants.Failed => StripePaymentMethodStatus.Failed,
            StripePaymentMethodStatusConstants.Confirmed => StripePaymentMethodStatus.Confirmed,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToStripePaymentMethodStatus(this StripePaymentMethodStatus src) =>
        src switch
        {
            StripePaymentMethodStatus.Pending => StripePaymentMethodStatusConstants.Pending,
            StripePaymentMethodStatus.Failed => StripePaymentMethodStatusConstants.Failed,
            StripePaymentMethodStatus.Confirmed => StripePaymentMethodStatusConstants.Confirmed,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToStripePaymentMethodStatusName(this StripePaymentMethodStatus src) =>
        src switch
        {
            StripePaymentMethodStatus.Pending => "Pending",
            StripePaymentMethodStatus.Failed => "Failed",
            StripePaymentMethodStatus.Confirmed => "Confirmed",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToStripePaymentMethodStatusName(this string src) =>
        src switch
        {
            StripePaymentMethodStatusConstants.Pending => "Pending",
            StripePaymentMethodStatusConstants.Failed => "Failed",
            StripePaymentMethodStatusConstants.Confirmed => "Confirmed",
            _ => throw new ArgumentOutOfRangeException()
        };
}
