using Enterprise.Shared.Cookie;
using Enterprise.Shared.Cookie.Configurations;
using Enterprise.Shared.Encryption;

namespace Enterprise.Shared.UnitTests.Cookie.CookieEncryptionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DecryptShould
{
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
