using Enterprise.Shared.Accounting.Configurations;
using Enterprise.Shared.Security;

namespace Enterprise.Shared.Accounting;

public interface IXeroTokenEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}

public class XeroTokenEncryptionService(XeroConfiguration xeroConfiguration, IStringEncryptionAlgorithm stringEncryptionAlgorithm)
    : IXeroTokenEncryptionService
{
    public string Encrypt(string plainText) => stringEncryptionAlgorithm.Encrypt(plainText, xeroConfiguration.EncryptionKey);
    public string Decrypt(string cipherText) => stringEncryptionAlgorithm.Decrypt(cipherText, xeroConfiguration.EncryptionKey);
}
