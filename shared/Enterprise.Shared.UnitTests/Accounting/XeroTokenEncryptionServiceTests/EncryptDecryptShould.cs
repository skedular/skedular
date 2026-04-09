using Enterprise.Shared.Accounting;
using Enterprise.Shared.Accounting.Configurations;
using Enterprise.Shared.Security;

namespace Enterprise.Shared.UnitTests.Accounting.XeroTokenEncryptionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EncryptDecryptShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Encrypt_delegates_to_algorithm(
        IStringEncryptionAlgorithm algorithm,
        XeroConfiguration xeroConfiguration,
        string plainText,
        string expectedCipherText)
    {
        A.CallTo(() => algorithm.Encrypt(plainText, xeroConfiguration.EncryptionKey)).Returns(expectedCipherText);

        var sut = new XeroTokenEncryptionService(xeroConfiguration, algorithm);

        sut.Encrypt(plainText).ShouldBe(expectedCipherText);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Decrypt_delegates_to_algorithm(
        IStringEncryptionAlgorithm algorithm,
        XeroConfiguration xeroConfiguration,
        string cipherText,
        string expectedPlainText)
    {
        A.CallTo(() => algorithm.Decrypt(cipherText, xeroConfiguration.EncryptionKey)).Returns(expectedPlainText);

        var sut = new XeroTokenEncryptionService(xeroConfiguration, algorithm);

        sut.Decrypt(cipherText).ShouldBe(expectedPlainText);
    }
}
