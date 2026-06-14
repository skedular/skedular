using Organization.Api.GraphQL.Pricing;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Api.UnitTests.GraphQL.Pricing.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class PricingCatalogChoicesShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_Product_Offering_Choices(RootQuery sut)
    {
        var result = sut.PricingCatalogProductOfferings().ToList();

        result.ShouldContain(item => item.Type == PricingCatalogProductOfferingCode.Teams && item.Name == "Teams");
        result.ShouldContain(item => item.Type == PricingCatalogProductOfferingCode.Spaces && item.Name == "Spaces");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Subscription_Status_Choices(RootQuery sut)
    {
        var result = sut.OrganizationOfferingPlanStatuses().ToList();

        result.ShouldContain(item => item.Type == OrganizationOfferingPlanStatus.Active && item.Name == "Active");
        result.ShouldContain(item => item.Type == OrganizationOfferingPlanStatus.Legacy && item.Name == "Legacy");
    }
}
