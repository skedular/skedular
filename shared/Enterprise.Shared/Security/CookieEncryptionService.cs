using Enterprise.Shared.Encryption;
using Enterprise.Shared.Security.Configurations;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Security;

public interface ICookieEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}

public class CookieEncryptionService(
    CookieConfiguration cookieConfiguration,
    IStringEncryptionAlgorithm stringEncryptionAlgorithm,
    ILogger<CookieEncryptionService> logger)
    : ICookieEncryptionService
{
    public string Encrypt(string plainText)
    {
        logger.LogDebug("Encrypting cookie payload. PayloadLength={PayloadLength}", plainText.Length);
        return stringEncryptionAlgorithm.Encrypt(plainText, cookieConfiguration.EncryptionKey);
    }

    public string Decrypt(string cipherText)
    {
        logger.LogDebug("Decrypting cookie payload. PayloadLength={PayloadLength}", cipherText.Length);
        return stringEncryptionAlgorithm.Decrypt(cipherText, cookieConfiguration.EncryptionKey);
    }
}
