using Enterprise.Shared.Security.Sso;

namespace Enterprise.Shared.UnitTests.Security.Sso.SamlLoginRequestFactoryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GenerateSamlLoginRequestShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_url_with_saml_request_and_relay_state(
        string id,
        string redirectUrl,
        string entityId)
    {
        var loginUrl = "https://idp.example.com/sso";
        var timeProvider = A.Fake<TimeProvider>();
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(new DateTimeOffset(2024, 6, 15, 10, 0, 0, TimeSpan.Zero));
        var sut = new SamlLoginRequestFactory(timeProvider);

        var result = sut.GenerateSamlLoginRequest(id, redirectUrl, entityId, loginUrl);

        result.ShouldContain("SAMLRequest=");
        result.ShouldContain("RelayState=");
        result.ShouldStartWith(loginUrl);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Throw_when_id_is_empty(string redirectUrl, string entityId, string loginUrl)
    {
        var sut = new SamlLoginRequestFactory(A.Fake<TimeProvider>());
        Should.Throw<ArgumentException>(() => sut.GenerateSamlLoginRequest("", redirectUrl, entityId, loginUrl));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Throw_when_redirect_url_is_empty(string id, string entityId, string loginUrl)
    {
        var sut = new SamlLoginRequestFactory(A.Fake<TimeProvider>());
        Should.Throw<ArgumentException>(() => sut.GenerateSamlLoginRequest(id, "", entityId, loginUrl));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Throw_when_entity_id_is_empty(string id, string redirectUrl, string loginUrl)
    {
        var sut = new SamlLoginRequestFactory(A.Fake<TimeProvider>());
        Should.Throw<ArgumentException>(() => sut.GenerateSamlLoginRequest(id, redirectUrl, "", loginUrl));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Throw_when_login_url_is_empty(string id, string redirectUrl, string entityId)
    {
        var sut = new SamlLoginRequestFactory(A.Fake<TimeProvider>());
        Should.Throw<ArgumentException>(() => sut.GenerateSamlLoginRequest(id, redirectUrl, entityId, ""));
    }
}
