using Api.Shared.Services.Offering;
using Organization.Api.GraphQL.Pricing;
using Organization.Api.Services.Pricing;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Api.UnitTests.GraphQL.Pricing.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class OrganizationSpacesSubscriptionShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Authoritative_Trial_State(
        [Frozen]
        IOrganizationSpacesSubscriptionService subscriptionService,
        RootQuery sut,
        string organizationId,
        DateTimeOffset trialStartedAt,
        CancellationToken cancellationToken)
    {
        var trialEndsAt = trialStartedAt.AddDays(14);
        A.CallTo(() => subscriptionService.GetAsync(organizationId, cancellationToken))
            .Returns(new OrganizationSpacesSubscription
            {
                Id = "subscription-1",
                Organization = new Shared.Models.Organization
                {
                    Id = organizationId,
                },
                PlanCode = PricingCatalogSubscriptionPlanCode.Free,
                SubscriptionStatus = SpacesSubscriptionStatus.TrialExpiring,
                AccessReason = SpacesAccessReasonCode.AllowedReadOrRecovery,
                TrialStartedAt = trialStartedAt,
                TrialEndsAt = trialEndsAt,
                RemainingTrialDays = 3,
                CanUseProduct = true,
                CanAcceptBookings = true,
                CanProtectExistingCommitments = true,
            });

        var result = await sut.OrganizationSpacesSubscriptionAsync(
            organizationId,
            subscriptionService,
            cancellationToken);

        result.ShouldNotBeNull();
        result.SubscriptionStatus.ShouldBe(SpacesSubscriptionStatus.TrialExpiring);
        result.TrialStartedAt.ShouldBe(trialStartedAt);
        result.TrialEndsAt.ShouldBe(trialEndsAt);
        result.RemainingTrialDays.ShouldBe(3);
        result.CanAcceptBookings.ShouldBeTrue();
        result.UpgradeRequired.ShouldBeFalse();
    }
}
