using Api.Shared.Services.Offering;

namespace Api.Shared.Services.UnitTests.Offering.OfferingsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetEffectiveHostCommissionPercentageShould
{
    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(12.5)]
    public void Waive_Commission_For_Early_Bird(decimal configuredPercentage)
    {
        OfferingCode.EarlyBirdV1.GetEffectiveHostCommissionPercentage(configuredPercentage).ShouldBe(0m);
    }

    [Fact]
    public void Preserve_Commission_For_Non_Early_Bird_Offering()
    {
        OfferingCode.HostStandardV1.GetEffectiveHostCommissionPercentage(5m).ShouldBe(5m);
    }
}
