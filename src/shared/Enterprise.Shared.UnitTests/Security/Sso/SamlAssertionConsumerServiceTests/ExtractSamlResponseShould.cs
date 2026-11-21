using Enterprise.Shared.Security.Sso;

namespace Enterprise.Shared.UnitTests.Security.Sso.SamlAssertionConsumerServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ExtractSamlResponseShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Extract_assertion_id(SamlAssertionConsumerService sut)
    {
        var result = sut.ExtractSamlResponse(CreateResponseXml("assertion-id"));

        result.AssertionId.ShouldBe("assertion-id");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Throw_when_assertion_id_is_missing(SamlAssertionConsumerService sut) =>
        Should.Throw<ArgumentException>(() => sut.ExtractSamlResponse(CreateResponseXml(null)));

    private static string CreateResponseXml(string? assertionId)
    {
        var assertionIdAttribute = assertionId is null ? string.Empty : $"ID=\"{assertionId}\"";
        return $"""
                <samlp:Response xmlns:samlp="urn:oasis:names:tc:SAML:2.0:protocol"
                                xmlns:saml="urn:oasis:names:tc:SAML:2.0:assertion"
                                Destination="https://app.example.com/sso"
                                InResponseTo="request-id">
                  <saml:Assertion {assertionIdAttribute}>
                    <saml:Issuer>https://idp.example.com</saml:Issuer>
                  </saml:Assertion>
                  <samlp:Status>
                    <samlp:StatusCode Value="urn:oasis:names:tc:SAML:2.0:status:Success" />
                  </samlp:Status>
                </samlp:Response>
                """;
    }
}
