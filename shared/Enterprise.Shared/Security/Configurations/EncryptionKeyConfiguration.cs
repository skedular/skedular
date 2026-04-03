namespace Enterprise.Shared.Security.Configurations;

public class EncryptionKeyConfiguration
{
    public string Key { get; set; } = string.Empty;
    public string Iv { get; set; } = string.Empty;
}
