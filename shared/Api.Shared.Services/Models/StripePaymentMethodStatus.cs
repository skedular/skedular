namespace Api.Shared.Services.Models;

public enum StripePaymentMethodStatus1
{
    Pending,
    Failed,
    Confirmed
}

public static class StripePaymentMethodStatus1Constants
{
    public const string Pending = "PENDING";
    public const string Failed = "FAILED";
    public const string Confirmed = "CONFIRMED";
}

public static class StripePaymentMethodStatus1Extensions
{
    public static StripePaymentMethodStatus1 ToStripePaymentMethodStatus1(this string src) =>
        src switch
        {
            StripePaymentMethodStatus1Constants.Pending => StripePaymentMethodStatus1.Pending,
            StripePaymentMethodStatus1Constants.Failed => StripePaymentMethodStatus1.Failed,
            StripePaymentMethodStatus1Constants.Confirmed => StripePaymentMethodStatus1.Confirmed,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToStripePaymentMethodStatus1(this StripePaymentMethodStatus1 src) =>
        src switch
        {
            StripePaymentMethodStatus1.Pending => StripePaymentMethodStatus1Constants.Pending,
            StripePaymentMethodStatus1.Failed => StripePaymentMethodStatus1Constants.Failed,
            StripePaymentMethodStatus1.Confirmed => StripePaymentMethodStatus1Constants.Confirmed,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToStripePaymentMethodStatus1Name(this StripePaymentMethodStatus1 src) =>
        src switch
        {
            StripePaymentMethodStatus1.Pending => "Pending",
            StripePaymentMethodStatus1.Failed => "Failed",
            StripePaymentMethodStatus1.Confirmed => "Confirmed",
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToStripePaymentMethodStatus1Name(this string src) =>
        src switch
        {
            StripePaymentMethodStatus1Constants.Pending => "Pending",
            StripePaymentMethodStatus1Constants.Failed => "Failed",
            StripePaymentMethodStatus1Constants.Confirmed => "Confirmed",
            _ => throw new ArgumentOutOfRangeException()
        };
}
