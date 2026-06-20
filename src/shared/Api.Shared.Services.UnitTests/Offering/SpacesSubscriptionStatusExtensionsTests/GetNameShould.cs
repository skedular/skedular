using Api.Shared.Services.Offering;

namespace Api.Shared.Services.UnitTests.Offering.SpacesSubscriptionStatusExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetNameShould
{
    [Theory]
    [InlineData(SpacesSubscriptionStatus.NotSet, "Not Set")]
    [InlineData(SpacesSubscriptionStatus.TrialActive, "Trial Active")]
    [InlineData(SpacesSubscriptionStatus.TrialExpiring, "Trial Expiring")]
    [InlineData(SpacesSubscriptionStatus.TrialExpired, "Trial Expired")]
    [InlineData(SpacesSubscriptionStatus.ComplimentaryBridge, "Complimentary Bridge")]
    [InlineData(SpacesSubscriptionStatus.PaidActive, "Paid Active")]
    [InlineData(SpacesSubscriptionStatus.PaidInactive, "Paid Inactive")]
    [InlineData(SpacesSubscriptionStatus.LegacyActive, "Legacy Active")]
    [InlineData(SpacesSubscriptionStatus.MissingState, "Missing State")]
    public void Return_Stable_Name(SpacesSubscriptionStatus status, string expected) =>
        status.ToSpacesSubscriptionStatusName().ShouldBe(expected);
}
