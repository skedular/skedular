using Enterprise.Shared.Encryption;
using Enterprise.Shared.Encryption.Configurations;

namespace Enterprise.Shared.UnitTests.Security.StringEncryptionAlgorithmTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EncryptDecryptShould
{
    private static EncryptionKeyConfiguration ValidKey => new()
    {
        Key = "12345678901234567890123456789012", // 32 bytes
        Iv = "1234567890123456" // 16 bytes
    };

    [Theory]
    [AutoFakeItEasyData]
    public void Round_trip_encrypt_and_decrypt(StringEncryptionAlgorithm sut, string plainText)
    {
        var encrypted = sut.Encrypt(plainText, ValidKey);
        var decrypted = sut.Decrypt(encrypted, ValidKey);

        decrypted.ShouldBe(plainText);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Encrypt_produces_different_ciphertexts_for_same_input(StringEncryptionAlgorithm sut, string plainText)
    {
        var encrypted1 = sut.Encrypt(plainText, ValidKey);
        var encrypted2 = sut.Encrypt(plainText, ValidKey);

        // GCM uses random nonce so should differ
        encrypted1.ShouldNotBe(encrypted2);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Encrypted_value_starts_with_version_prefix(StringEncryptionAlgorithm sut, string plainText)
    {
        var encrypted = sut.Encrypt(plainText, ValidKey);

        encrypted.ShouldStartWith("v2:");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Encrypt_throws_when_key_is_null(StringEncryptionAlgorithm sut, string plainText) =>
        Should.Throw<ArgumentNullException>(() => sut.Encrypt(plainText, null!));

    [Theory]
    [AutoFakeItEasyData]
    public void Decrypt_throws_when_key_is_null(StringEncryptionAlgorithm sut, string cipherText) =>
        Should.Throw<ArgumentNullException>(() => sut.Decrypt(cipherText, null!));

    [Theory]
    [AutoFakeItEasyData]
    public void Decrypt_throws_when_cipher_text_is_whitespace(StringEncryptionAlgorithm sut) =>
        Should.Throw<ArgumentException>(() => sut.Decrypt("   ", ValidKey));

    [Theory]
    [AutoFakeItEasyData]
    public void Encrypt_throws_when_key_is_wrong_length(StringEncryptionAlgorithm sut, string plainText)
    {
        var badKey = new EncryptionKeyConfiguration { Key = "tooshort", Iv = "1234567890123456" };
        Should.Throw<ArgumentException>(() => sut.Encrypt(plainText, badKey));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Encrypt_throws_when_iv_is_wrong_length(StringEncryptionAlgorithm sut, string plainText)
    {
        var badKey = new EncryptionKeyConfiguration { Key = "12345678901234567890123456789012", Iv = "tooshort" };
        Should.Throw<ArgumentException>(() => sut.Encrypt(plainText, badKey));
    }
}
