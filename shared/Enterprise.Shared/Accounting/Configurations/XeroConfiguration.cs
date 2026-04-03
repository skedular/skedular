using Enterprise.Shared.Security.Configurations;

namespace Enterprise.Shared.Accounting.Configurations;

public class XeroConfiguration
{
    public const string Key = "Xero";

    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string AuthorizeEndpoint { get; set; } = string.Empty;
    public string TokenEndpoint { get; set; } = string.Empty;
    public string WebhookKey { get; set; } = string.Empty;
    public bool LogWebhookMessages { get; set; }
    public string Scopes { get; set; } = string.Empty;
    public int RefreshBeforeExpiryDays { get; set; }
    public EncryptionKeyConfiguration EncryptionKey { get; set; } = new();
}
