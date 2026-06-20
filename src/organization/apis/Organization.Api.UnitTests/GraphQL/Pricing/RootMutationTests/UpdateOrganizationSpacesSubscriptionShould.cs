using Organization.Api.GraphQL.Pricing;
using Organization.Api.Services.Pricing;
using Organization.Shared.Models.PricingCatalog;
using PricingCatalogSubscriptionPlanCodeAlias = Organization.Shared.Models.PricingCatalog.PricingCatalogSubscriptionPlanCode;

namespace Organization.Api.UnitTests.GraphQL.Pricing.RootMutationTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdateOrganizationSpacesSubscriptionShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Call_Service_UpdateAsync_For_Growth(
        [Frozen] IOrganizationSpacesSubscriptionService subscriptionService,
        RootMutation sut,
        CancellationToken cancellationToken)
    {
        var input = new UpdateOrganizationSpacesSubscriptionInput
        {
            OrganizationId = "org-1",
            PlanCode = PricingCatalogSubscriptionPlanCodeAlias.Growth,
            CustomCapacity = null,
            ClientMutationId = "test-1"
        };

        A.CallTo(() => subscriptionService.UpdateAsync(
                input.OrganizationId, input.PlanCode, input.CustomCapacity, A<CancellationToken>._))
            .Returns(Task.FromResult(new OrganizationSpacesSubscription()));

        var result = await sut.UpdateOrganizationSpacesSubscriptionAsync(input, subscriptionService, cancellationToken);

        A.CallTo(() => subscriptionService.UpdateAsync(
                input.OrganizationId, input.PlanCode, input.CustomCapacity, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        result.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Call_Service_UpdateAsync_For_Business(
        [Frozen] IOrganizationSpacesSubscriptionService subscriptionService,
        RootMutation sut,
        CancellationToken cancellationToken)
    {
        var input = new UpdateOrganizationSpacesSubscriptionInput
        {
            OrganizationId = "org-2",
            PlanCode = PricingCatalogSubscriptionPlanCodeAlias.Business,
            CustomCapacity = null,
            ClientMutationId = "test-2"
        };

        A.CallTo(() => subscriptionService.UpdateAsync(
                input.OrganizationId, input.PlanCode, input.CustomCapacity, A<CancellationToken>._))
            .Returns(Task.FromResult(new OrganizationSpacesSubscription()));

        var result = await sut.UpdateOrganizationSpacesSubscriptionAsync(input, subscriptionService, cancellationToken);

        result.ShouldNotBeNull();
        result.ClientMutationId.ShouldBe("test-2");
    }
}
