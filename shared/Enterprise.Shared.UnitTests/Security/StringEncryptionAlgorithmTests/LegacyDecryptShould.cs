using Enterprise.Shared.Security;
using Enterprise.Shared.Security.Configurations;

namespace Enterprise.Shared.UnitTests.Security.StringEncryptionAlgorithmTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class LegacyDecryptShould
{
    private static EncryptionKeyConfiguration ValidKey => new()
    {
        Key = "12345678901234567890123456789012", // 32 bytes
        Iv = "1234567890123456"  // 16 bytes
    };

    [Fact]
    public void Decrypt_legacy_cbc_ciphertext()
    {
        // Produce a legacy (AES-CBC) encrypted value to test the legacy path.
        // We use the internal legacy format: Base58 of AES-CBC encrypted bytes (no "v2:" prefix).
        var sut = new StringEncryptionAlgorithm();

        // First encrypt using v2 to get the key/iv shapes, then manually create a legacy token.
        // Build a legacy token the same way the old code would have produced it.
        var plainText = "hello legacy";
        var key = System.Text.Encoding.UTF8.GetBytes(ValidKey.Key);
        var iv = System.Text.Encoding.UTF8.GetBytes(ValidKey.Iv);
        using var aes = System.Security.Cryptography.Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        var encryptor = aes.CreateEncryptor();
        using var ms = new System.IO.MemoryStream();
        using (var cs = new System.Security.Cryptography.CryptoStream(ms, encryptor, System.Security.Cryptography.CryptoStreamMode.Write))
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            cs.Write(bytes, 0, bytes.Length);
        }
        var cipherBytes = ms.ToArray();
        var legacyCipherText = SimpleBase.Base58.Bitcoin.Encode(cipherBytes);

        // Should not start with "v2:" so it will use DecryptLegacy
        legacyCipherText.ShouldNotStartWith("v2:");

        var result = sut.Decrypt(legacyCipherText, ValidKey);

        result.ShouldBe(plainText);
    }
}
