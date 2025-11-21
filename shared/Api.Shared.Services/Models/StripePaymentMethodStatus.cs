namespace Api.Shared.Services.Models;

public enum StripePaymentMethodStatus
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
    extension(string src)
    {
        public StripePaymentMethodStatus ToStripePaymentMethodStatus() =>
            src switch
            {
                StripePaymentMethodStatus1Constants.Pending => StripePaymentMethodStatus.Pending,
                StripePaymentMethodStatus1Constants.Failed => StripePaymentMethodStatus.Failed,
                StripePaymentMethodStatus1Constants.Confirmed => StripePaymentMethodStatus.Confirmed,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToStripePaymentMethodStatusName() =>
            src switch
            {
                StripePaymentMethodStatus1Constants.Pending => "Pending",
                StripePaymentMethodStatus1Constants.Failed => "Failed",
                StripePaymentMethodStatus1Constants.Confirmed => "Confirmed",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(StripePaymentMethodStatus src)
    {
        public string ToStripePaymentMethodStatus() =>
            src switch
            {
                StripePaymentMethodStatus.Pending => StripePaymentMethodStatus1Constants.Pending,
                StripePaymentMethodStatus.Failed => StripePaymentMethodStatus1Constants.Failed,
                StripePaymentMethodStatus.Confirmed => StripePaymentMethodStatus1Constants.Confirmed,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToStripePaymentMethodStatusName() =>
            src switch
            {
                StripePaymentMethodStatus.Pending => "Pending",
                StripePaymentMethodStatus.Failed => "Failed",
                StripePaymentMethodStatus.Confirmed => "Confirmed",
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
