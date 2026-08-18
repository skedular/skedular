using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Services.Cache;
using Booking.Shared.Services.Entitlements;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Entitlement;

[QueryType]
public sealed class RootQuery(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<IReadOnlyList<EntitlementDetails>> MyEntitlementsAsync(
        [Service]
        ICachedCustomerService cachedCustomerService,
        [Service]
        IEntitlementService entitlementService,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var entitlements = await entitlementService.GetForCustomerAsync(customerId, cancellationToken);
        return [.. entitlements.Select(graphQlMapper.MapTo)];
    }

    [UseResolverScope]
    public async Task<EntitlementDetails?> EntitlementAsync(
        string id,
        [Service]
        IEntitlementReadService entitlementReadService,
        [Service]
        ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken) =>
        await entitlementReadService.GetAuthorizedAsync(id, await cachedCustomerService.GetIdAsync(cancellationToken), cancellationToken) is
            { } entitlement
            ? graphQlMapper.MapTo(entitlement)
            : null;

    [UseResolverScope]
    public async Task<IReadOnlyList<EntitlementDetails>> EntitlementsByCustomerAsync(
        string customerId,
        [Service]
        IEntitlementReadService entitlementReadService,
        [Service]
        ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken)
    {
        var entitlements = await entitlementReadService.GetForCustomerAsync(customerId,
            await cachedCustomerService.GetIdAsync(cancellationToken), cancellationToken);
        return [.. entitlements.Select(graphQlMapper.MapTo)];
    }

    [UseResolverScope]
    public async Task<IReadOnlyList<EntitlementDetails>> OrganizationEntitlementsAsync(
        string organizationId,
        [Service]
        IEntitlementReadService entitlementReadService,
        [Service]
        ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var entitlements = await entitlementReadService.GetForOrganizationAsync(organizationId, customerId, cancellationToken);
        return [.. entitlements.Select(graphQlMapper.MapTo)];
    }
}
