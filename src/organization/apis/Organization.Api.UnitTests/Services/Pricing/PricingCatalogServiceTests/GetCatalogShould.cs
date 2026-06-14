using Organization.Api.Services.Pricing;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Api.UnitTests.Services.Pricing.PricingCatalogServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetCatalogShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_Teams_Catalog_With_Free_Pay_As_You_Go_And_Enterprise_Capacity(PricingCatalogService sut)
    {
        var result = sut.GetCatalog(PricingCatalogProductOfferingCode.Teams);
        var teams = result.ProductOfferings.Single();

        teams.Code.ShouldBe(PricingCatalogProductOfferingCode.Teams);
        teams.Plans.Select(plan => plan.Code).ShouldBe([
            PricingCatalogSubscriptionPlanCode.Free,
            PricingCatalogSubscriptionPlanCode.PayAsYouGo,
            PricingCatalogSubscriptionPlanCode.EnterpriseCapacity
        ]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Enterprise_As_Contact_Us_Custom_Capacity(PricingCatalogService sut)
    {
        var enterprise = sut.GetCatalog(PricingCatalogProductOfferingCode.Teams)
            .ProductOfferings.Single()
            .Plans.Single(plan => plan.Code == PricingCatalogSubscriptionPlanCode.EnterpriseCapacity);

        enterprise.Availability.ShouldBe(PricingCatalogPlanAvailability.ContactUs);
        var capacityOption = enterprise.CapacityOptions.Single();
        capacityOption.UserCapacity.ShouldBeNull();
        capacityOption.Price.ShouldBeNull();
        capacityOption.Availability.ShouldBe(PricingCatalogPlanAvailability.ContactUs);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_All_Product_Offerings_When_Filter_Is_Not_Set(PricingCatalogService sut)
    {
        var result = sut.GetCatalog(PricingCatalogProductOfferingCode.NotSet);

        result.ProductOfferings.Select(offering => offering.Code).ShouldBe([
            PricingCatalogProductOfferingCode.Teams,
            PricingCatalogProductOfferingCode.Spaces
        ]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Spaces_Framework_Catalog(PricingCatalogService sut)
    {
        var result = sut.GetCatalog(PricingCatalogProductOfferingCode.Spaces);
        var spaces = result.ProductOfferings.Single();

        spaces.Code.ShouldBe(PricingCatalogProductOfferingCode.Spaces);
        spaces.Plans.Single().Code.ShouldBe(PricingCatalogSubscriptionPlanCode.EnterpriseCapacity);
        spaces.Plans.Single().Availability.ShouldBe(PricingCatalogPlanAvailability.ContactUs);
        spaces.Plans.Single().Limits.ShouldNotContain(limit => limit.Code == "monthly-active-users");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Teams_Only_When_Filtered_To_Teams(PricingCatalogService sut)
    {
        var result = sut.GetCatalog(PricingCatalogProductOfferingCode.Teams);

        result.ProductOfferings.Single().Code.ShouldBe(PricingCatalogProductOfferingCode.Teams);
    }
}
