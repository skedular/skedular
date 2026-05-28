using Customer.Shared.Services;

namespace Customer.Shared.UnitTests.Services;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RequiredCustomerReadinessDomainServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_All_Confirmed_Required_Domains(RequiredCustomerReadinessDomainService sut)
    {
        var requiredDomains = sut.GetRequiredDomains();

        requiredDomains.Count.ShouldBe(8);
        requiredDomains.ShouldContain("Booking");
        requiredDomains.ShouldContain("Organization");
        requiredDomains.ShouldContain("Team");
        requiredDomains.ShouldContain("Marketplace");
        requiredDomains.ShouldContain("Location");
        requiredDomains.ShouldContain("Core");
        requiredDomains.ShouldContain("Slack");
        requiredDomains.ShouldContain("MsTeams");
    }
}
