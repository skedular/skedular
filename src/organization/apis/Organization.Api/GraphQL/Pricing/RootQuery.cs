using Api.Shared.Services.Offering;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services.Pricing;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Api.GraphQL.Pricing;

[QueryType]
public class RootQuery
{
    [UseResolverScope]
    public PricingCatalogDetails PricingCatalog(
        PricingCatalogProductOfferingCode? productOfferingCode,
        [Service] IPricingCatalogService pricingCatalogService,
        [Service] IGraphQlMapper graphQlMapper) =>
        graphQlMapper.MapTo(pricingCatalogService.GetCatalog(productOfferingCode));

    [UseResolverScope]
    public async Task<OrganizationTeamsSubscriptionDetails?> OrganizationTeamsSubscriptionAsync(
        string organizationId,
        [Service] IOrganizationTeamsSubscriptionService organizationTeamsSubscriptionService,
        CancellationToken cancellationToken)
    {
        var subscription = await organizationTeamsSubscriptionService.GetAsync(organizationId, cancellationToken);
        return subscription is null
            ? null
            : new OrganizationTeamsSubscriptionDetails
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

    [UseResolverScope]
    public IEnumerable<PricingCatalogProductOfferingDetails> PricingCatalogProductOfferings() =>
    [
        new()
        {
            Type = PricingCatalogProductOfferingCode.Teams,
            Name = PricingCatalogProductOfferingCode.Teams.ToPricingCatalogProductOfferingCodeName()
        },
        new()
        {
            Type = PricingCatalogProductOfferingCode.Spaces,
            Name = PricingCatalogProductOfferingCode.Spaces.ToPricingCatalogProductOfferingCodeName()
        }
    ];

    [UseResolverScope]
    public IEnumerable<PricingCatalogSubscriptionPlanDetails> PricingCatalogSubscriptionPlans() =>
    [
        new()
        {
            Type = PricingCatalogSubscriptionPlanCode.Free,
            Name = PricingCatalogSubscriptionPlanCode.Free.ToPricingCatalogSubscriptionPlanCodeName()
        },
        new()
        {
            Type = PricingCatalogSubscriptionPlanCode.PayAsYouGo,
            Name = PricingCatalogSubscriptionPlanCode.PayAsYouGo.ToPricingCatalogSubscriptionPlanCodeName()
        },
        new()
        {
            Type = PricingCatalogSubscriptionPlanCode.EnterpriseCapacity,
            Name = PricingCatalogSubscriptionPlanCode.EnterpriseCapacity.ToPricingCatalogSubscriptionPlanCodeName()
        },
        new()
        {
            Type = PricingCatalogSubscriptionPlanCode.LegacyEarlyBird,
            Name = PricingCatalogSubscriptionPlanCode.LegacyEarlyBird.ToPricingCatalogSubscriptionPlanCodeName()
        }
    ];

    [UseResolverScope]
    public IEnumerable<PricingCatalogPlanAvailabilityDetails> PricingCatalogPlanAvailabilities() =>
    [
        new()
        {
            Type = PricingCatalogPlanAvailability.SelfService,
            Name = PricingCatalogPlanAvailability.SelfService.ToPricingCatalogPlanAvailabilityName()
        },
        new()
        {
            Type = PricingCatalogPlanAvailability.ContactUs,
            Name = PricingCatalogPlanAvailability.ContactUs.ToPricingCatalogPlanAvailabilityName()
        },
        new() { Type = PricingCatalogPlanAvailability.Hidden, Name = PricingCatalogPlanAvailability.Hidden.ToPricingCatalogPlanAvailabilityName() },
        new()
        {
            Type = PricingCatalogPlanAvailability.Deprecated,
            Name = PricingCatalogPlanAvailability.Deprecated.ToPricingCatalogPlanAvailabilityName()
        },
        new()
        {
            Type = PricingCatalogPlanAvailability.Unavailable,
            Name = PricingCatalogPlanAvailability.Unavailable.ToPricingCatalogPlanAvailabilityName()
        },
        new()
        {
            Type = PricingCatalogPlanAvailability.ExistingCustomersOnly,
            Name = PricingCatalogPlanAvailability.ExistingCustomersOnly.ToPricingCatalogPlanAvailabilityName()
        }
    ];

    [UseResolverScope]
    public IEnumerable<OrganizationOfferingPlanStatusDetails> OrganizationOfferingPlanStatuses() =>
    [
        new() { Type = OrganizationOfferingPlanStatus.Pending, Name = OrganizationOfferingPlanStatus.Pending.ToOrganizationOfferingPlanStatusName() },
        new() { Type = OrganizationOfferingPlanStatus.Active, Name = OrganizationOfferingPlanStatus.Active.ToOrganizationOfferingPlanStatusName() },
        new()
        {
            Type = OrganizationOfferingPlanStatus.ScheduledChange,
            Name = OrganizationOfferingPlanStatus.ScheduledChange.ToOrganizationOfferingPlanStatusName()
        },
        new()
        {
            Type = OrganizationOfferingPlanStatus.Canceled, Name = OrganizationOfferingPlanStatus.Canceled.ToOrganizationOfferingPlanStatusName()
        },
        new() { Type = OrganizationOfferingPlanStatus.Expired, Name = OrganizationOfferingPlanStatus.Expired.ToOrganizationOfferingPlanStatusName() },
        new() { Type = OrganizationOfferingPlanStatus.Legacy, Name = OrganizationOfferingPlanStatus.Legacy.ToOrganizationOfferingPlanStatusName() }
    ];

    [UseResolverScope]
    public IEnumerable<PricingEntitlementReasonCodeDetails> PricingEntitlementReasonCodes() =>
        Enum.GetValues<EntitlementReasonCode>()
            .Where(reasonCode => reasonCode != EntitlementReasonCode.NotSet)
            .Select(reasonCode => new PricingEntitlementReasonCodeDetails { Type = reasonCode, Name = reasonCode.ToEntitlementReasonCodeName() });
}
