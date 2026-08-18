using Api.Shared.Services.Offering;
using SharedOffering = Api.Shared.Services.Models.Offering;

namespace Api.Shared.Services.UnitTests.Offering.PricingEntitlementEvaluatorTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EvaluateActiveUserShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Allow_Pay_As_You_Go_Active_User(PricingEntitlementEvaluator sut, string customerId)
    {
        var offering = new SharedOffering
        {
            Code = OfferingCode.PayAsYouGoV1,
            ActiveCustomerIds = [.. Enumerable.Range(0, 50).Select(index => $"customer-{index}")],
        };

        var result = sut.EvaluateActiveUser(offering, customerId);

        result.IsAllowed.ShouldBeTrue();
        result.ReasonCode.ShouldBe(EntitlementReasonCode.Allowed);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Block_Free_New_Active_User_When_Limit_Reached(PricingEntitlementEvaluator sut, string customerId)
    {
        var offering = new SharedOffering
        {
            Code = OfferingCode.FreeTierV1,
            PurchasedUserCapacity = 10,
            ActiveCustomerIds = [.. Enumerable.Range(0, 10).Select(index => $"customer-{index}")],
        };

        var result = sut.EvaluateActiveUser(offering, customerId);

        result.IsAllowed.ShouldBeFalse();
        result.ReasonCode.ShouldBe(EntitlementReasonCode.FreeActiveUserLimitReached);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Allow_Existing_Active_User_When_Limit_Reached(PricingEntitlementEvaluator sut)
    {
        var offering = new SharedOffering
        {
            Code = OfferingCode.EnterpriseCustomV1,
            PurchasedUserCapacity = 2,
            ActiveCustomerIds = ["customer-1", "customer-2"],
        };

        var result = sut.EvaluateActiveUser(offering, "customer-2");

        result.IsAllowed.ShouldBeTrue();
        result.ReasonCode.ShouldBe(EntitlementReasonCode.Allowed);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Block_Enterprise_New_Active_User_When_Capacity_Reached(PricingEntitlementEvaluator sut)
    {
        var offering = new SharedOffering
        {
            Code = OfferingCode.EnterpriseCustomV1,
            PurchasedUserCapacity = 2,
            ActiveCustomerIds = ["customer-1", "customer-2"],
        };

        var result = sut.EvaluateActiveUser(offering, "customer-3");

        result.IsAllowed.ShouldBeFalse();
        result.ReasonCode.ShouldBe(EntitlementReasonCode.EnterpriseCapacityReached);
    }
}
