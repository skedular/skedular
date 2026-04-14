using Enterprise.Shared.Accounting.Configurations;
using Enterprise.Shared.Security;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Accounting;

public interface IXeroTokenEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}

public class XeroTokenEncryptionService(
    XeroConfiguration xeroConfiguration,
    IStringEncryptionAlgorithm stringEncryptionAlgorithm,
    ILogger<XeroTokenEncryptionService> logger)
    : IXeroTokenEncryptionService
{
    public string Encrypt(string plainText)
    {
        logger.LogDebug("Encrypting Xero token payload. PayloadLength={PayloadLength}", plainText.Length);
        return stringEncryptionAlgorithm.Encrypt(plainText, xeroConfiguration.EncryptionKey);
    }

    public string Decrypt(string cipherText)
    {
        logger.LogDebug("Decrypting Xero token payload. PayloadLength={PayloadLength}", cipherText.Length);
        return stringEncryptionAlgorithm.Decrypt(cipherText, xeroConfiguration.EncryptionKey);
    }
}
