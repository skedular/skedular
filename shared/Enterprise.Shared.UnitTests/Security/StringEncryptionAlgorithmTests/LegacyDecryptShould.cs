using System.Security.Cryptography;
using System.Text;
using Enterprise.Shared.Security;
using Enterprise.Shared.Security.Configurations;
using SimpleBase;

namespace Enterprise.Shared.UnitTests.Security.StringEncryptionAlgorithmTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class LegacyDecryptShould
{
    private static EncryptionKeyConfiguration ValidKey => new()
    {
        Key = "12345678901234567890123456789012", // 32 bytes
        Iv = "1234567890123456" // 16 bytes
    };

    [Theory]
    [AutoFakeItEasyData]
    public void Decrypt_legacy_cbc_ciphertext(StringEncryptionAlgorithm sut)
    {
        // First encrypt using v2 to get the key/iv shapes, then manually create a legacy token.
        // Build a legacy token the same way the old code would have produced it.
        const string PlainText = "hello legacy";

        var key = Encoding.UTF8.GetBytes(ValidKey.Key);
        var iv = Encoding.UTF8.GetBytes(ValidKey.Iv);
        using var aes = Aes.Create();

        aes.Key = key;
        aes.IV = iv;
        var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            var bytes = Encoding.UTF8.GetBytes(PlainText);
            cs.Write(bytes, 0, bytes.Length);
        }

        var cipherBytes = ms.ToArray();
        var legacyCipherText = Base58.Bitcoin.Encode(cipherBytes);

        // Should not start with "v2:" so it will use DecryptLegacy
        legacyCipherText.ShouldNotStartWith("v2:");

        var result = sut.Decrypt(legacyCipherText, ValidKey);

        result.ShouldBe(PlainText);
    }
}
