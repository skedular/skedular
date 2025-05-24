namespace Enterprise.Shared.Security.Configurations;

public class CookieConfiguration
{
    public const string Key = "Cookie";

    public CookieEncryptionKey EncryptionKey { get; set; } = new();
}

public class CookieEncryptionKey
{
    public string Key { get; set; } = string.Empty;
    public string Iv { get; set; } = string.Empty;
}
