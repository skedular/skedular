using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Services.Pricing;
using OrganizationOfferingPlanDto = Organization.Shared.Models.PricingCatalog.OrganizationOfferingPlan;
using OrganizationSpacesSubscriptionDto = Organization.Shared.Models.PricingCatalog.OrganizationSpacesSubscription;

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

    [UseResolverScope]
    public async Task<UpdateOrganizationSpacesSubscriptionPayload> UpdateOrganizationSpacesSubscriptionAsync(
        UpdateOrganizationSpacesSubscriptionInput input,
        [Service] IOrganizationSpacesSubscriptionService organizationSpacesSubscriptionService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            OrganizationSpacesSubscription = MapTo(await organizationSpacesSubscriptionService.UpdateAsync(
                input.OrganizationId,
                input.PlanCode,
                input.CustomCapacity,
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

    private static OrganizationSpacesSubscriptionDetails MapTo(OrganizationSpacesSubscriptionDto subscription) =>
        new()
        {
            Id = subscription.Id,
            OrganizationId = subscription.Organization.Id,
            PlanCode = subscription.PlanCode,
            CommercialModel = subscription.CommercialModel,
            CurrentPeriodStartUtc = subscription.CurrentPeriodStart,
            CurrentPeriodEndUtc = subscription.CurrentPeriodEnd,
            UsageLimit = subscription.UsageLimit,
            RolloverDate = subscription.RolloverDate,
            CustomCapacity = subscription.CustomCapacity,
            CatalogVersionCode = subscription.CatalogVersion,
            Status = subscription.Status,
            SubscriptionStatus = subscription.SubscriptionStatus,
            AccessReason = subscription.AccessReason,
            TrialStartedAt = subscription.TrialStartedAt,
            TrialEndsAt = subscription.TrialEndsAt,
            RemainingTrialDays = subscription.RemainingTrialDays,
            CanUseProduct = subscription.CanUseProduct,
            CanAcceptBookings = subscription.CanAcceptBookings,
            CanProtectExistingCommitments = subscription.CanProtectExistingCommitments,
            UpgradeRequired = subscription.UpgradeRequired,
            IsComplimentaryBridge = subscription.IsComplimentaryBridge,
            NextBillingAt = subscription.NextBillingAt,
            CreatedAt = subscription.CreatedAt,
            UpdatedAt = subscription.ModifiedAt ?? subscription.CreatedAt
        };
}
