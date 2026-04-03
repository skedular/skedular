namespace Enterprise.Shared.Security.Configurations;

public class CookieConfiguration
{
    public const string Key = "Cookie";

    public EncryptionKeyConfiguration EncryptionKey { get; set; } = new();
}
