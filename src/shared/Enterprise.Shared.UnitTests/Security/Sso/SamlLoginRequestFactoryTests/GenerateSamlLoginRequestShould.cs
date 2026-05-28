using Enterprise.Shared.Security.Sso;

namespace Enterprise.Shared.UnitTests.Security.Sso.SamlLoginRequestFactoryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GenerateSamlLoginRequestShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_url_with_saml_request_and_relay_state(
        [Frozen] TimeProvider timeProvider,
        SamlLoginRequestFactory sut,
        string id,
        string redirectUrl,
        string entityId)
    {
        const string LoginUrl = "https://idp.example.com/sso";
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(new DateTimeOffset(2024, 6, 15, 10, 0, 0, TimeSpan.Zero));

        var result = sut.GenerateSamlLoginRequest(id, redirectUrl, entityId, LoginUrl);

        result.ShouldContain("SAMLRequest=");
        result.ShouldContain("RelayState=");
        result.ShouldStartWith(LoginUrl);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Throw_when_id_is_empty(SamlLoginRequestFactory sut, string redirectUrl, string entityId, string loginUrl) =>
        Should.Throw<ArgumentException>(() => sut.GenerateSamlLoginRequest(string.Empty, redirectUrl, entityId, loginUrl));

    [Theory]
    [AutoFakeItEasyData]
    public void Throw_when_redirect_url_is_empty(SamlLoginRequestFactory sut, string id, string entityId, string loginUrl) =>
        Should.Throw<ArgumentException>(() => sut.GenerateSamlLoginRequest(id, string.Empty, entityId, loginUrl));

    [Theory]
    [AutoFakeItEasyData]
    public void Throw_when_entity_id_is_empty(SamlLoginRequestFactory sut, string id, string redirectUrl, string loginUrl) =>
        Should.Throw<ArgumentException>(() => sut.GenerateSamlLoginRequest(id, redirectUrl, string.Empty, loginUrl));

    [Theory]
    [AutoFakeItEasyData]
    public void Throw_when_login_url_is_empty(SamlLoginRequestFactory sut, string id, string redirectUrl, string entityId) =>
        Should.Throw<ArgumentException>(() => sut.GenerateSamlLoginRequest(id, redirectUrl, entityId, string.Empty));
}
