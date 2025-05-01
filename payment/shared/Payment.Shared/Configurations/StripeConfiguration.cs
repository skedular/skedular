namespace Payment.Shared.Configurations;

public class StripeConfiguration
{
    public const string Key = "Stripe";

    public string PublishableKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string PlatformAccountWebhookKey { get; set; } = string.Empty;
    public string ConnectAccountWebhookKey { get; set; } = string.Empty;
    public bool RemoveStripeConnectAccountFromStripe { get; set; }
    public bool LogStripPlatformAccountWebhookMessages { get; set; }
    public bool LogStripeConnectAccountWebhookMessages { get; set; }
}
