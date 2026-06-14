using Api.Shared.Services.Offering;
using SharedOffering = Api.Shared.Services.Models.Offering;

namespace Api.Shared.Services.UnitTests.Offering.PricingEntitlementEvaluatorTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EvaluateLocationCreationShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Block_Free_Second_Location(PricingEntitlementEvaluator sut)
    {
        var offering = new SharedOffering { Code = OfferingCode.FreeTierV1, PurchasedLocationCapacity = 1 };

        var result = sut.EvaluateLocationCreation(offering, 1);

        result.IsAllowed.ShouldBeFalse();
        result.ReasonCode.ShouldBe(EntitlementReasonCode.FreeLocationLimitReached);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Allow_Enterprise_Location_Creation(PricingEntitlementEvaluator sut)
    {
        var offering = new SharedOffering { Code = OfferingCode.EnterpriseCustomV1, PurchasedLocationCapacity = -1 };

        var result = sut.EvaluateLocationCreation(offering, 25);

        result.IsAllowed.ShouldBeTrue();
        result.ReasonCode.ShouldBe(EntitlementReasonCode.Allowed);
    }
}
