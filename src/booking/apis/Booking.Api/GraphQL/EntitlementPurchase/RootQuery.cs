using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Services.Cache;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.EntitlementPurchase;

[QueryType]
public sealed class RootQuery
{
    [UseResolverScope]
    public async Task<EntitlementPurchaseDetails?> EntitlementPurchaseAsync(
        string purchaseId,
        [Service]
        ICachedCustomerService cachedCustomerService,
        [Service]
        IEntitlementPurchaseReadService entitlementPurchaseReadService,
        [Service]
        IEntitlementPurchaseGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var purchase = await entitlementPurchaseReadService.GetAuthorizedAsync(purchaseId, customerId, cancellationToken);
        return purchase is null ? null : graphQlMapper.Map(purchase);
    }

    [UseResolverScope]
    public async Task<IReadOnlyList<EntitlementPurchaseDetails>> EntitlementPurchasesAsync(
        [Service]
        ICachedCustomerService cachedCustomerService,
        [Service]
        IEntitlementPurchaseReadService entitlementPurchaseReadService,
        [Service]
        IEntitlementPurchaseGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var purchases = await entitlementPurchaseReadService.GetForCustomerAsync(customerId, cancellationToken);
        return [.. purchases.Select(graphQlMapper.Map)];
    }

    [UseResolverScope]
    public async Task<IReadOnlyList<EntitlementPurchaseDetails>> OrganizationEntitlementPurchasesAsync(
        string organizationId,
        [Service]
        ICachedCustomerService cachedCustomerService,
        [Service]
        IEntitlementPurchaseReadService entitlementPurchaseReadService,
        [Service]
        IEntitlementPurchaseGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var purchases = await entitlementPurchaseReadService.GetForOrganizationAsync(organizationId, customerId, cancellationToken);
        return [.. purchases.Select(graphQlMapper.Map)];
    }
}
