using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Services.Pricing;
using OrganizationOfferingPlanDto = Organization.Shared.Models.PricingCatalog.OrganizationOfferingPlan;

namespace Organization.Api.GraphQL.Pricing;

[MutationType]
public class RootMutation
{
    [UseResolverScope]
    public async Task<UpdateOrganizationTeamsSubscriptionPayload> UpdateOrganizationTeamsSubscriptionAsync(
        UpdateOrganizationTeamsSubscriptionInput input,
        [Service] IOrganizationTeamsSubscriptionService organizationTeamsSubscriptionService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            OrganizationTeamsSubscription = MapTo(await organizationTeamsSubscriptionService.UpdateAsync(
                input.OrganizationId,
                input.PlanCode,
                input.PurchasedUserCapacity,
                input.PurchasedLocationCapacity,
                input.PurchasedTeamCapacity,
                cancellationToken))
        };

    private static OrganizationTeamsSubscriptionDetails MapTo(OrganizationOfferingPlanDto subscription) =>
        new()
        {
            Id = subscription.Id,
            OrganizationId = subscription.OrganizationId,
            ProductOfferingCode = subscription.ProductOfferingCode,
            PlanCode = subscription.PlanCode,
            UnitPrice = subscription.UnitPrice,
            FixedPrice = subscription.FixedPrice,
            Currency = subscription.Currency,
            PurchasedUserCapacity = subscription.PurchasedUserCapacity,
            PurchasedLocationCapacity = subscription.PurchasedLocationCapacity,
            PurchasedTeamCapacity = subscription.PurchasedTeamCapacity,
            CatalogVersionCode = subscription.CatalogVersionCode,
            Status = subscription.Status,
            EffectiveFrom = subscription.EffectiveFrom,
            EffectiveUntil = subscription.EffectiveUntil,
            AutoRenew = subscription.AutoRenew
        };
}
