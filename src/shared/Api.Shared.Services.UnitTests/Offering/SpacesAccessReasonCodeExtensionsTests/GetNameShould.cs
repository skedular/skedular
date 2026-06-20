using Api.Shared.Services.Offering;

namespace Api.Shared.Services.UnitTests.Offering.SpacesAccessReasonCodeExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetNameShould
{
    [Theory]
    [InlineData(SpacesAccessReasonCode.NotSet, "Not Set")]
    [InlineData(SpacesAccessReasonCode.AllowedTrial, "Allowed Trial")]
    [InlineData(SpacesAccessReasonCode.AllowedPaid, "Allowed Paid")]
    [InlineData(SpacesAccessReasonCode.AllowedComplimentaryBridge, "Allowed Complimentary Bridge")]
    [InlineData(SpacesAccessReasonCode.AllowedProtectiveAction, "Allowed Protective Action")]
    [InlineData(SpacesAccessReasonCode.AllowedReadOrRecovery, "Allowed Read or Recovery")]
    [InlineData(SpacesAccessReasonCode.TrialExpired, "Trial Expired")]
    [InlineData(SpacesAccessReasonCode.PaidInactive, "Paid Inactive")]
    [InlineData(SpacesAccessReasonCode.MissingTrialState, "Missing Trial State")]
    [InlineData(SpacesAccessReasonCode.MissingOfferingState, "Missing Offering State")]
    [InlineData(SpacesAccessReasonCode.ActionNotAllowed, "Action Not Allowed")]
    public void Return_Stable_Name(SpacesAccessReasonCode reasonCode, string expected) =>
        reasonCode.ToSpacesAccessReasonCodeName().ShouldBe(expected);
}
