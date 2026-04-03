using System.Security.Cryptography;
using System.Text;
using Enterprise.Shared.Security.Configurations;
using SimpleBase;

namespace Enterprise.Shared.Security;

public interface IStringEncryptionAlgorithm
{
    string Encrypt(string plainText, EncryptionKeyConfiguration encryptionKey);
    string Decrypt(string cipherText, EncryptionKeyConfiguration encryptionKey);
}

public class StringEncryptionAlgorithm : IStringEncryptionAlgorithm
{
    private const string VersionPrefix = "v2:";
    private const int NonceLength = 12;
    private const int TagLength = 16;

    public string Encrypt(string plainText, EncryptionKeyConfiguration encryptionKey)
    {
        ArgumentNullException.ThrowIfNull(encryptionKey);
        var (key, _) = GetKeyAndIv(encryptionKey);
        var nonce = RandomNumberGenerator.GetBytes(NonceLength);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = new byte[plainBytes.Length];
        var tag = new byte[TagLength];

        using var aesGcm = new AesGcm(key, TagLength);
        aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);

        var payloadBytes = new byte[NonceLength + TagLength + cipherBytes.Length];
        Buffer.BlockCopy(nonce, 0, payloadBytes, 0, NonceLength);
        Buffer.BlockCopy(tag, 0, payloadBytes, NonceLength, TagLength);
        Buffer.BlockCopy(cipherBytes, 0, payloadBytes, NonceLength + TagLength, cipherBytes.Length);

        return $"{VersionPrefix}{Base58.Bitcoin.Encode(payloadBytes)}";
    }

    public string Decrypt(string cipherText, EncryptionKeyConfiguration encryptionKey)
    {
        ArgumentNullException.ThrowIfNull(encryptionKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(cipherText);

        return cipherText.StartsWith(VersionPrefix, StringComparison.Ordinal)
            ? DecryptVersion2(cipherText[VersionPrefix.Length..], encryptionKey)
            : DecryptLegacy(cipherText, encryptionKey);
    }

    private static string DecryptVersion2(string cipherText, EncryptionKeyConfiguration encryptionKey)
    {
        var (key, _) = GetKeyAndIv(encryptionKey);
        var payloadBytes = Base58.Bitcoin.Decode(cipherText);
        if (payloadBytes.Length < NonceLength + TagLength)
        {
            throw new CryptographicException("Cipher text payload is invalid.");
        }

        var nonce = payloadBytes[..NonceLength];
        var tag = payloadBytes[NonceLength..(NonceLength + TagLength)];
        var cipherBytes = payloadBytes[(NonceLength + TagLength)..];
        var plainBytes = new byte[cipherBytes.Length];

        using var aesGcm = new AesGcm(key, TagLength);
        aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);

        return Encoding.UTF8.GetString(plainBytes);
    }

    private static string DecryptLegacy(string cipherText, EncryptionKeyConfiguration encryptionKey)
    {
        var (key, iv) = GetKeyAndIv(encryptionKey);
        var cipherBytes = Base58.Bitcoin.Decode(cipherText);

        using var aesAlg = Aes.Create();
        aesAlg.Key = key;
        aesAlg.IV = iv;

        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        using var msDecrypt = new MemoryStream(cipherBytes);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        return srDecrypt.ReadToEnd();
    }

    private static (byte[] Key, byte[] Iv) GetKeyAndIv(EncryptionKeyConfiguration encryptionKey)
    {
        var key = Encoding.UTF8.GetBytes(encryptionKey.Key);
        if (key.Length != 32)
        {
            throw new ArgumentException($"{nameof(encryptionKey.Key)} must be 32 bytes.");
        }

        var iv = Encoding.UTF8.GetBytes(encryptionKey.Iv);
        if (iv.Length != 16)
        {
            throw new ArgumentException($"{nameof(encryptionKey.Iv)} must be 16 bytes.");
        }

        return (key, iv);
    }
}
