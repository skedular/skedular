using Booking.Api.Services.Authorization;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Services.Entitlements;

namespace Booking.Api.Services;

public interface IEntitlementPurchaseReadService
{
    Task<IReadOnlyList<EntitlementPurchase>> GetForCustomerAsync(string customerId, CancellationToken cancellationToken);

    Task<IReadOnlyList<EntitlementPurchase>> GetForOrganizationAsync(string organizationId, string customerId,
        CancellationToken cancellationToken);

    Task<EntitlementPurchase?> GetAuthorizedAsync(string purchaseId, string customerId, CancellationToken cancellationToken);
    Task<bool> CanCreateAsync(string organizationId, string customerId, CancellationToken cancellationToken);
}

public sealed class EntitlementPurchaseReadService(
    IEntitlementPurchaseService entitlementPurchaseService,
    IOrganizationAuthorizationService organizationAuthorizationService) : IEntitlementPurchaseReadService
{
    public Task<IReadOnlyList<EntitlementPurchase>> GetForCustomerAsync(string customerId, CancellationToken cancellationToken) =>
        entitlementPurchaseService.GetForCustomerAsync(customerId, cancellationToken);

    public async Task<IReadOnlyList<EntitlementPurchase>> GetForOrganizationAsync(string organizationId, string customerId,
        CancellationToken cancellationToken)
    {
        if (!await organizationAuthorizationService.CanOverrideCancellationPolicyAsync(organizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return await entitlementPurchaseService.GetForOrganizationAsync(organizationId, cancellationToken);
    }

    public async Task<EntitlementPurchase?> GetAuthorizedAsync(string purchaseId, string customerId, CancellationToken cancellationToken)
    {
        var purchase = await entitlementPurchaseService.GetByIdAsync(purchaseId, cancellationToken);
        if (purchase is null)
        {
            return null;
        }

        if (string.Equals(purchase.CustomerId, customerId, StringComparison.Ordinal))
        {
            return purchase;
        }

        if (!await organizationAuthorizationService.CanModifyPaymentMethodAsync(purchase.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return purchase;
    }

    public Task<bool> CanCreateAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        return Task.FromResult(true);
    }
}
