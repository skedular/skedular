namespace Enterprise.Shared.Payment.Configurations;

public class StripeConfiguration
{
    public const string Key = "Stripe";

    public string PublishableKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string OrganizationPlatformAccountWebhookKey { get; set; } = string.Empty;
    public string OrganizationConnectAccountWebhookKey { get; set; } = string.Empty;
    public string BookingPlatformAccountWebhookKey { get; set; } = string.Empty;
    public string BookingConnectAccountWebhookKey { get; set; } = string.Empty;
    public bool LogStripPlatformAccountWebhookMessages { get; set; }
    public bool LogStripeConnectAccountWebhookMessages { get; set; }
    public string OAuthClientId { get; set; } = string.Empty;
}
