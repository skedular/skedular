using Enterprise.Shared.Cookie;
using Enterprise.Shared.Cookie.Configurations;
using Enterprise.Shared.Encryption;

namespace Enterprise.Shared.UnitTests.Cookie.CookieEncryptionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EncryptShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Encrypt_delegates_to_algorithm(
        [Frozen]
        IStringEncryptionAlgorithm algorithm,
        [Frozen]
        CookieConfiguration cookieConfiguration,
        CookieEncryptionService sut,
        string plainText,
        string expectedCipherText)
    {
        A.CallTo(() => algorithm.Encrypt(plainText, cookieConfiguration.EncryptionKey)).Returns(expectedCipherText);

        sut.Encrypt(plainText).ShouldBe(expectedCipherText);
    }
}
