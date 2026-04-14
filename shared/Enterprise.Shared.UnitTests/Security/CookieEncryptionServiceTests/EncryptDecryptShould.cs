using Enterprise.Shared.Security;
using Enterprise.Shared.Security.Configurations;

namespace Enterprise.Shared.UnitTests.Security.CookieEncryptionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EncryptDecryptShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Encrypt_delegates_to_algorithm(
        [Frozen] IStringEncryptionAlgorithm algorithm,
        [Frozen] CookieConfiguration cookieConfiguration,
        CookieEncryptionService sut,
        string plainText,
        string expectedCipherText)
    {
        A.CallTo(() => algorithm.Encrypt(plainText, cookieConfiguration.EncryptionKey)).Returns(expectedCipherText);

        sut.Encrypt(plainText).ShouldBe(expectedCipherText);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Decrypt_delegates_to_algorithm(
        [Frozen] IStringEncryptionAlgorithm algorithm,
        [Frozen] CookieConfiguration cookieConfiguration,
        CookieEncryptionService sut,
        string cipherText,
        string expectedPlainText)
    {
        A.CallTo(() => algorithm.Decrypt(cipherText, cookieConfiguration.EncryptionKey)).Returns(expectedPlainText);

        sut.Decrypt(cipherText).ShouldBe(expectedPlainText);
    }
}
