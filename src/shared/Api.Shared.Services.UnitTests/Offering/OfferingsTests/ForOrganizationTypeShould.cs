using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;

namespace Api.Shared.Services.UnitTests.Offering.OfferingsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ForOrganizationTypeShould
{
    [Theory]
    [InlineData(OrganizationType.Private)]
    public void Return_Teams_Offerings_For_Private_Organization(OrganizationType organizationType) =>
        Offerings.ForOrganizationType(organizationType).ShouldBe(Offerings.TeamsOfferings);

    [Theory]
    [InlineData(OrganizationType.Marketplace)]
    public void Return_Spaces_Offerings_For_Marketplace_Organization(OrganizationType organizationType) =>
        Offerings.ForOrganizationType(organizationType).ShouldBe(Offerings.SpacesOfferings);

    [Theory]
    [InlineData(OrganizationType.Host)]
    public void Return_Host_Offerings_For_Host_Organization(OrganizationType organizationType) =>
        Offerings.ForOrganizationType(organizationType).ShouldBe(Offerings.HostOfferings);

    [Fact]
    public void Throw_For_Unsupported_Organization_Type()
    {
        var exception = Should.Throw<ArgumentOutOfRangeException>(() => Offerings.ForOrganizationType((OrganizationType)999));

        exception.ParamName.ShouldBe("organizationType");
    }
}
