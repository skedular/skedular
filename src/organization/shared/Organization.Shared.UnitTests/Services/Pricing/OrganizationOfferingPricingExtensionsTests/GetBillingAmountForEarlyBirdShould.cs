using Api.Shared.Services.Offering;
using Organization.Shared.Database.Entities;
using Organization.Shared.Services.Pricing;

namespace Organization.Shared.UnitTests.Services.Pricing.OrganizationOfferingPricingExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetBillingAmountForEarlyBirdShould
{
    [Fact]
    public void Return_Zero_Despite_Persisted_Price()
    {
        var offering = new OrganizationOffering
        {
            Code = OfferingCode.EarlyBirdV1,
            FixedPrice = 10_000,
            UnitPrice = 500,
            DiscountPercentage = 0
        };

        offering.GetBillingAmount().ShouldBe(0);
    }
}
