using System.Security.Cryptography;
using System.Text;
using Enterprise.Shared.Configurations;
using SimpleBase;

namespace Enterprise.Shared.Security;

public interface ICookieHelper
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}

public class CookieHelper : ICookieHelper
{
    private readonly byte[] _iv;
    private readonly byte[] _key;

    public CookieHelper(CookieConfiguration cookieConfiguration)
    {
        _key = Encoding.UTF8.GetBytes(cookieConfiguration.EncryptionKey.Key);
        if (_key.Length != 32)
        {
            throw new ArgumentException($"{nameof(cookieConfiguration.EncryptionKey.Key)} must be 32 bytes.");
        }

        _iv = Encoding.UTF8.GetBytes(cookieConfiguration.EncryptionKey.Iv);
        if (_iv.Length != 16)
        {
            throw new ArgumentException($"{nameof(cookieConfiguration.EncryptionKey.Iv)} must be 16 bytes.");
        }
    }

    public string Encrypt(string plainText)
    {
        using var aesAlg = Aes.Create();
        aesAlg.Key = _key;
        aesAlg.IV = _iv;

        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }
        }

        return Base58.Bitcoin.Encode(msEncrypt.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        var cipherBytes = Base58.Bitcoin.Decode(cipherText);

        using var aesAlg = Aes.Create();
        aesAlg.Key = _key;
        aesAlg.IV = _iv;

        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        using var msDecrypt = new MemoryStream(cipherBytes);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        return srDecrypt.ReadToEnd();
    }
}
