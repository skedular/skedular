using Api.Shared.Services.Offering;
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

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Spaces_Subscription_Status_And_Reason_Choices(RootQuery sut)
    {
        var statuses = sut.SpacesSubscriptionStatuses().ToList();
        var reasons = sut.SpacesAccessReasons().ToList();

        statuses.ShouldContain(item => item.Type == SpacesSubscriptionStatus.TrialActive && item.Name == "Trial Active");
        statuses.ShouldContain(item => item.Type == SpacesSubscriptionStatus.TrialExpired && item.Name == "Trial Expired");
        reasons.ShouldContain(item => item.Type == SpacesAccessReasonCode.AllowedTrial && item.Name == "Allowed Trial");
        reasons.ShouldContain(item => item.Type == SpacesAccessReasonCode.TrialExpired && item.Name == "Trial Expired");
    }
}
