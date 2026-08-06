using Enterprise.Shared.Accounting;
using Enterprise.Shared.Accounting.Configurations;
using Enterprise.Shared.Encryption;

namespace Enterprise.Shared.UnitTests.Accounting.XeroTokenEncryptionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EncryptDecryptShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Encrypt_delegates_to_algorithm(
        [Frozen]
        IStringEncryptionAlgorithm algorithm,
        [Frozen]
        XeroConfiguration xeroConfiguration,
        XeroTokenEncryptionService sut,
        string plainText,
        string expectedCipherText)
    {
        A.CallTo(() => algorithm.Encrypt(plainText, xeroConfiguration.EncryptionKey)).Returns(expectedCipherText);

        sut.Encrypt(plainText).ShouldBe(expectedCipherText);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Decrypt_delegates_to_algorithm(
        [Frozen]
        IStringEncryptionAlgorithm algorithm,
        [Frozen]
        XeroConfiguration xeroConfiguration,
        XeroTokenEncryptionService sut,
        string cipherText,
        string expectedPlainText)
    {
        A.CallTo(() => algorithm.Decrypt(cipherText, xeroConfiguration.EncryptionKey)).Returns(expectedPlainText);

        sut.Decrypt(cipherText).ShouldBe(expectedPlainText);
    }
}
