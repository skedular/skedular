using Api.Shared.Services.Offering;
using SharedOffering = Api.Shared.Services.Models.Offering;

namespace Api.Shared.Services.UnitTests.Offering.SpacesAccessEvaluatorTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EvaluateShould
{
    private static readonly DateTimeOffset s_trialStart = new(2026, 7, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset s_trialEnd = s_trialStart.AddDays(14);

    [Theory]
    [InlineAutoFakeItEasyData(new Type[] { }, SpacesAccessAction.Read)]
    [InlineAutoFakeItEasyData(new Type[] { }, SpacesAccessAction.CreateOrModify)]
    [InlineAutoFakeItEasyData(new Type[] { }, SpacesAccessAction.CreateBookingInstance)]
    [InlineAutoFakeItEasyData(new Type[] { }, SpacesAccessAction.ProtectExistingCommitment)]
    [InlineAutoFakeItEasyData(new Type[] { }, SpacesAccessAction.AccountOrUpgrade)]
    public void Allow_Every_Action_During_Active_Trial(SpacesAccessAction action, SpacesAccessEvaluator sut)
    {
        var result = sut.Evaluate(s_trialStart, TrialOffering(), action);

        result.Allowed.ShouldBeTrue();
        result.Status.ShouldBe(SpacesSubscriptionStatus.TrialActive);
        result.RemainingTrialDays.ShouldBe(14);
        result.CanAcceptBookings.ShouldBeTrue();
        result.UpgradeRequired.ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Enter_Expiring_Status_When_Three_Whole_Days_Remain(SpacesAccessEvaluator sut)
    {
        var result = sut.Evaluate(s_trialEnd.AddDays(-3), TrialOffering(), SpacesAccessAction.CreateOrModify);

        result.Status.ShouldBe(SpacesSubscriptionStatus.TrialExpiring);
        result.RemainingTrialDays.ShouldBe(3);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Round_Remaining_Days_Up(SpacesAccessEvaluator sut)
    {
        var result = sut.Evaluate(s_trialEnd.AddHours(-1), TrialOffering(), SpacesAccessAction.Read);

        result.RemainingTrialDays.ShouldBe(1);
    }

    [Theory]
    [InlineAutoFakeItEasyData(new Type[] { }, SpacesAccessAction.CreateOrModify)]
    [InlineAutoFakeItEasyData(new Type[] { }, SpacesAccessAction.CreateBookingInstance)]
    public void Deny_New_Work_At_Exact_Expiry(SpacesAccessAction action, SpacesAccessEvaluator sut)
    {
        var result = sut.Evaluate(s_trialEnd, TrialOffering(), action);

        result.Allowed.ShouldBeFalse();
        result.Status.ShouldBe(SpacesSubscriptionStatus.TrialExpired);
        result.ReasonCode.ShouldBe(SpacesAccessReasonCode.TrialExpired);
        result.RemainingTrialDays.ShouldBe(0);
        result.UpgradeRequired.ShouldBeTrue();
    }

    [Theory]
    [InlineAutoFakeItEasyData(new Type[] { }, SpacesAccessAction.Read, SpacesAccessReasonCode.AllowedReadOrRecovery)]
    [InlineAutoFakeItEasyData(new Type[] { }, SpacesAccessAction.AccountOrUpgrade, SpacesAccessReasonCode.AllowedReadOrRecovery)]
    [InlineAutoFakeItEasyData(new Type[] { }, SpacesAccessAction.ProtectExistingCommitment, SpacesAccessReasonCode.AllowedProtectiveAction)]
    public void Allow_Recovery_And_Protective_Actions_After_Expiry(
        SpacesAccessAction action,
        SpacesAccessReasonCode expectedReason,
        SpacesAccessEvaluator sut)
    {
        var result = sut.Evaluate(s_trialEnd, TrialOffering(), action);

        result.Allowed.ShouldBeTrue();
        result.ReasonCode.ShouldBe(expectedReason);
        result.CanUseProduct.ShouldBeFalse();
        result.CanProtectExistingCommitments.ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Use_Offering_Start_For_Legacy_Free_Projection(SpacesAccessEvaluator sut)
    {
        var offering = TrialOffering();
        offering.SpacesTrialStartedAt = null;
        offering.SpacesTrialEndsAt = null;

        var result = sut.Evaluate(s_trialStart, offering, SpacesAccessAction.CreateBookingInstance);

        result.Allowed.ShouldBeTrue();
        result.Status.ShouldBe(SpacesSubscriptionStatus.TrialActive);
        result.TrialStartedAt.ShouldBe(offering.Start);
        result.TrialEndsAt.ShouldBe(offering.Start.Add(SpacesAccessEvaluator.TrialDuration));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Fail_Closed_When_Spaces_Product_Flag_Is_Disabled(SpacesAccessEvaluator sut)
    {
        var offering = TrialOffering();
        offering.SpacesProductEnabled = false;

        var result = sut.Evaluate(s_trialStart, offering, SpacesAccessAction.CreateBookingInstance);

        result.Allowed.ShouldBeFalse();
        result.Status.ShouldBe(SpacesSubscriptionStatus.MissingState);
        result.ReasonCode.ShouldBe(SpacesAccessReasonCode.MissingOfferingState);
    }

    [Theory]
    [InlineAutoFakeItEasyData(new Type[] { }, SpacesAccessAction.Read)]
    [InlineAutoFakeItEasyData(new Type[] { }, SpacesAccessAction.ProtectExistingCommitment)]
    [InlineAutoFakeItEasyData(new Type[] { }, SpacesAccessAction.AccountOrUpgrade)]
    public void Preserve_Safe_Actions_For_Legacy_Free_Projection(
        SpacesAccessAction action,
        SpacesAccessEvaluator sut)
    {
        var offering = TrialOffering();
        offering.SpacesTrialStartedAt = null;
        offering.SpacesTrialEndsAt = null;

        var result = sut.Evaluate(s_trialStart, offering, action);

        result.Allowed.ShouldBeTrue();
        result.CanUseProduct.ShouldBeTrue();
        result.CanProtectExistingCommitments.ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Prefer_Paid_Access_Over_Expired_Trial_History(SpacesAccessEvaluator sut)
    {
        var offering = new SharedOffering
        {
            Code = OfferingCode.SpacesGrowthV1,
            Start = s_trialEnd,
            End = s_trialEnd.AddMonths(1),
            SpacesProductEnabled = true,
            SpacesTrialStartedAt = s_trialStart,
            SpacesTrialEndsAt = s_trialEnd
        };

        var result = sut.Evaluate(s_trialEnd, offering, SpacesAccessAction.CreateBookingInstance);

        result.Allowed.ShouldBeTrue();
        result.Status.ShouldBe(SpacesSubscriptionStatus.PaidActive);
        result.ReasonCode.ShouldBe(SpacesAccessReasonCode.AllowedPaid);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Identify_Complimentary_Bridge(SpacesAccessEvaluator sut)
    {
        var billingStartsAt = new DateTimeOffset(2026, 8, 1, 0, 0, 0, TimeSpan.Zero);
        var offering = new SharedOffering
        {
            Code = OfferingCode.SpacesGrowthV1,
            Start = s_trialEnd,
            End = billingStartsAt,
            SpacesProductEnabled = true,
            SpacesTrialStartedAt = s_trialStart,
            SpacesTrialEndsAt = s_trialEnd,
            SpacesNextBillingAt = billingStartsAt
        };

        var result = sut.Evaluate(s_trialEnd, offering, SpacesAccessAction.CreateOrModify);

        result.Status.ShouldBe(SpacesSubscriptionStatus.ComplimentaryBridge);
        result.IsComplimentaryBridge.ShouldBeTrue();
        result.NextBillingAt.ShouldBe(billingStartsAt);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Leave_Teams_Offering_Unrestricted(SpacesAccessEvaluator sut)
    {
        var offering = new SharedOffering { Code = OfferingCode.PayAsYouGoV1, Start = s_trialStart, End = s_trialEnd, SpacesProductEnabled = false };

        var result = sut.Evaluate(s_trialEnd, offering, SpacesAccessAction.CreateOrModify);

        result.Allowed.ShouldBeTrue();
        result.Status.ShouldBe(SpacesSubscriptionStatus.LegacyActive);
    }

    private static SharedOffering TrialOffering() =>
        new()
        {
            Code = OfferingCode.SpacesFreeTierV1,
            Start = s_trialStart,
            End = s_trialEnd,
            SpacesProductEnabled = true,
            SpacesTrialStartedAt = s_trialStart,
            SpacesTrialEndsAt = s_trialEnd
        };
}
