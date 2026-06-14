using Api.Shared.Services.Offering;
using SharedOffering = Api.Shared.Services.Models.Offering;

namespace Api.Shared.Services.UnitTests.Offering.PricingEntitlementEvaluatorTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EvaluateTeamCreationShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Block_Free_Second_Team(PricingEntitlementEvaluator sut)
    {
        var offering = new SharedOffering { Code = OfferingCode.FreeTierV1, PurchasedTeamCapacity = 1 };

        var result = sut.EvaluateTeamCreation(offering, 1);

        result.IsAllowed.ShouldBeFalse();
        result.ReasonCode.ShouldBe(EntitlementReasonCode.FreeTeamLimitReached);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Allow_Pay_As_You_Go_Team_Creation(PricingEntitlementEvaluator sut)
    {
        var offering = new SharedOffering { Code = OfferingCode.PayAsYouGoV1, PurchasedTeamCapacity = -1 };

        var result = sut.EvaluateTeamCreation(offering, 10);

        result.IsAllowed.ShouldBeTrue();
        result.ReasonCode.ShouldBe(EntitlementReasonCode.Allowed);
    }
}
