using Enterprise.Shared.Security.Configurations;

namespace Enterprise.Shared.Security;

public interface ICookieEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}

public class CookieEncryptionService(CookieConfiguration cookieConfiguration, IStringEncryptionAlgorithm stringEncryptionAlgorithm)
    : ICookieEncryptionService
{
    public string Encrypt(string plainText) => stringEncryptionAlgorithm.Encrypt(plainText, cookieConfiguration.EncryptionKey);
    public string Decrypt(string cipherText) => stringEncryptionAlgorithm.Decrypt(cipherText, cookieConfiguration.EncryptionKey);
}
