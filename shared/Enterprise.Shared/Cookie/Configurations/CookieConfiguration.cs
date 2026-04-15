using Enterprise.Shared.Encryption.Configurations;

namespace Enterprise.Shared.Cookie.Configurations;

public class CookieConfiguration
{
    public const string Key = "Cookie";

    public EncryptionKeyConfiguration EncryptionKey { get; set; } = new();
}
