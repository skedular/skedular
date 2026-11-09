using Enterprise.Shared.Security.Sso;

namespace Enterprise.Shared.UnitTests.Security.Sso.SamlAssertionReplayCacheTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class TryRegisterShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Reject_the_same_assertion_for_the_same_issuer(
        SamlAssertionReplayCache sut,
        string issuer,
        string assertionId,
        DateTimeOffset now)
    {
        sut.TryRegister(issuer, assertionId, now).ShouldBeTrue();

        sut.TryRegister(issuer, assertionId, now).ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Allow_the_same_assertion_id_for_different_issuers(
        SamlAssertionReplayCache sut,
        string issuer,
        string otherIssuer,
        string assertionId,
        DateTimeOffset now)
    {
        otherIssuer.ShouldNotBe(issuer);
        sut.TryRegister(issuer, assertionId, now).ShouldBeTrue();

        sut.TryRegister(otherIssuer, assertionId, now).ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Allow_an_expired_assertion_id_to_be_reused(
        SamlAssertionReplayCache sut,
        string issuer,
        string assertionId,
        DateTimeOffset now)
    {
        sut.TryRegister(issuer, assertionId, now).ShouldBeTrue();

        sut.TryRegister(issuer, assertionId, now.AddHours(25)).ShouldBeTrue();
    }
}
