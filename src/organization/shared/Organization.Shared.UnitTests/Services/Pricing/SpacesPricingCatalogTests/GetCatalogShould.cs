using Organization.Shared.Models.PricingCatalog;
using Organization.Shared.Services.Pricing;

namespace Organization.Shared.UnitTests.Services.Pricing.SpacesPricingCatalogTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetCatalogShould
{
    [Fact]
    public void Describe_Free_As_Fourteen_Day_Trial_With_Existing_Booking_Limit()
    {
        var plan = SpacesPricingCatalogProvider.GetSpacesOffering().Plans
            .Single(item => item.Code == PricingCatalogSubscriptionPlanCode.Free);

        plan.Name.ShouldBe("14-day free trial");
        plan.Description.ShouldContain("14 days");
        plan.Features.ShouldContain(item => item.Code == "trial-period");
        plan.Features.ShouldContain(item => item.Code == "monthly-quota");
        plan.Limits.Single(item => item.Code == "booking-instances").Limit.ShouldBe(100);
    }
}
