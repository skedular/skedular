namespace Payment.Shared.Configurations;

public class StripeConfiguration
{
    public const string Key = "Stripe";

    public string PublishableKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
}
