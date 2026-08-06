using Organization.Api.Services.Pricing;
using Organization.Api.UnitTests.Fixtures;
using Organization.Shared.Models.PricingCatalog;
using Organization.Shared.Services.Pricing;

namespace Organization.Api.UnitTests.Services.Pricing.PricingCatalogServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetCatalogShould
{
    [Theory]
    [AutoFakeItEasyData([typeof(PricingCatalogServiceFixtureCustomizer)])]
    public void Return_Teams_Catalog_With_Free_Pay_As_You_Go_And_Enterprise_Capacity(PricingCatalogService sut)
    {
        var result = sut.GetCatalog(PricingCatalogProductOfferingCode.Teams);
        var teams = result.ProductOfferings.Single();

        teams.Code.ShouldBe(PricingCatalogProductOfferingCode.Teams);
        teams.Plans.Select(plan => plan.Code).ShouldBe([
            PricingCatalogSubscriptionPlanCode.Free,
            PricingCatalogSubscriptionPlanCode.PayAsYouGo,
            PricingCatalogSubscriptionPlanCode.EnterpriseCapacity,
        ]);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(PricingCatalogServiceFixtureCustomizer)])]
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
    [AutoFakeItEasyData([typeof(PricingCatalogServiceFixtureCustomizer)])]
    public void Return_All_Product_Offerings_When_Filter_Is_Not_Set(PricingCatalogService sut)
    {
        var result = sut.GetCatalog(PricingCatalogProductOfferingCode.NotSet);

        result.ProductOfferings.Select(offering => offering.Code).ShouldBe([
            PricingCatalogProductOfferingCode.Teams,
            PricingCatalogProductOfferingCode.Spaces,
            PricingCatalogProductOfferingCode.Host,
        ]);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(PricingCatalogServiceFixtureCustomizer)])]
    public void Return_Spaces_Framework_Catalog(PricingCatalogService sut)
    {
        var result = sut.GetCatalog(PricingCatalogProductOfferingCode.Spaces);
        var spaces = result.ProductOfferings.Single();

        spaces.Code.ShouldBe(PricingCatalogProductOfferingCode.Spaces);
        spaces.Plans.Select(plan => plan.Code).ShouldBe([
            PricingCatalogSubscriptionPlanCode.Free,
            PricingCatalogSubscriptionPlanCode.Growth,
            PricingCatalogSubscriptionPlanCode.Business,
            PricingCatalogSubscriptionPlanCode.ContactUs,
        ]);
        spaces.Plans.ShouldAllBe(plan => plan.Limits.All(limit => limit.Code != "monthly-active-users"));
    }

    [Theory]
    [AutoFakeItEasyData([typeof(PricingCatalogServiceFixtureCustomizer)])]
    public void Return_Teams_Only_When_Filtered_To_Teams(PricingCatalogService sut)
    {
        var result = sut.GetCatalog(PricingCatalogProductOfferingCode.Teams);

        result.ProductOfferings.Single().Code.ShouldBe(PricingCatalogProductOfferingCode.Teams);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(PricingCatalogServiceFixtureCustomizer)])]
    public void Return_Spaces_Catalog_Version_When_Filtered_To_Spaces(PricingCatalogService sut)
    {
        var result = sut.GetCatalog(PricingCatalogProductOfferingCode.Spaces);

        result.ActiveVersion.Code.ShouldBe(PricingCatalogConstants.CurrentSpacesCatalogVersion);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(PricingCatalogServiceFixtureCustomizer)])]
    public void Return_Host_Offering_And_Version_When_Filtered_To_Host(PricingCatalogService sut)
    {
        var result = sut.GetCatalog(PricingCatalogProductOfferingCode.Host);

        result.ActiveVersion.Code.ShouldBe(PricingCatalogConstants.CurrentHostCatalogVersion);
        var host = result.ProductOfferings.Single();
        host.Code.ShouldBe(PricingCatalogProductOfferingCode.Host);
        host.Plans.Single().Name.ShouldBe("Host Standard");
    }

    [Fact]
    public void Spaces_Free_Plan_Has_Correct_Catalog_Properties()
    {
        var offering = SpacesPricingCatalogProvider.GetSpacesOffering();
        var freePlan = offering.Plans[0];
        freePlan.Code.ShouldBe(PricingCatalogSubscriptionPlanCode.Free);
        freePlan.DisplayOrder.ShouldBe(1);
        freePlan.Recommended.ShouldBeFalse();
        freePlan.Name.ShouldBe("14-day free trial");
        var bookingLimit = freePlan.Limits.Single(l => l.Code == "booking-instances");
        bookingLimit.Limit.ShouldBe(100);
        bookingLimit.Unlimited.ShouldBeFalse();
    }

    [Fact]
    public void Spaces_Growth_Plan_Has_Correct_Catalog_Properties()
    {
        var offering = SpacesPricingCatalogProvider.GetSpacesOffering();
        var growthPlan = offering.Plans[1];
        growthPlan.Code.ShouldBe(PricingCatalogSubscriptionPlanCode.Growth);
        growthPlan.DisplayOrder.ShouldBe(2);
        growthPlan.Recommended.ShouldBeTrue();
        growthPlan.Limits.ShouldContain(l => l.Code == "booking-instances" && l.Limit == 500);
    }

    [Fact]
    public void Spaces_Business_Plan_Has_Correct_Catalog_Properties()
    {
        var offering = SpacesPricingCatalogProvider.GetSpacesOffering();
        var businessPlan = offering.Plans[2];
        businessPlan.Code.ShouldBe(PricingCatalogSubscriptionPlanCode.Business);
        businessPlan.DisplayOrder.ShouldBe(3);
        businessPlan.Limits.ShouldContain(l => l.Code == "booking-instances" && l.Limit == 1000);
    }

    [Fact]
    public void Spaces_ContactUs_Plan_Has_ContactUs_Availability()
    {
        var offering = SpacesPricingCatalogProvider.GetSpacesOffering();
        var contactUsPlan = offering.Plans[3];
        contactUsPlan.Code.ShouldBe(PricingCatalogSubscriptionPlanCode.ContactUs);
        contactUsPlan.Availability.ShouldBe(PricingCatalogPlanAvailability.ContactUs);
    }
}
