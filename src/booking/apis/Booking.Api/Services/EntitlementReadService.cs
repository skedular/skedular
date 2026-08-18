using Booking.Api.Services.Authorization;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Services.Entitlements;

namespace Booking.Api.Services;

public interface IEntitlementReadService
{
    Task<EntitlementModel?> GetAuthorizedAsync(string entitlementId, string customerId, CancellationToken cancellationToken);
    Task<IReadOnlyList<EntitlementModel>> GetForCustomerAsync(string customerId, string requestingCustomerId, CancellationToken cancellationToken);
    Task<IReadOnlyList<EntitlementModel>> GetForOrganizationAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    Task<EntitlementModel> GetAuthorizedForAdjustmentAsync(string entitlementId, string customerId, CancellationToken cancellationToken);
    Task<EntitlementModel> GetAuthorizedForCancellationAsync(string entitlementId, string customerId, CancellationToken cancellationToken);
    Task<EntitlementModel> GetAuthorizedForRenewalAsync(string entitlementId, string customerId, CancellationToken cancellationToken);
}

public sealed class EntitlementReadService(
    IEntitlementService entitlementService,
    IOrganizationAuthorizationService organizationAuthorizationService) : IEntitlementReadService
{
    public async Task<EntitlementModel?> GetAuthorizedAsync(string entitlementId, string customerId, CancellationToken cancellationToken)
    {
        var entitlement = await entitlementService.GetByIdAsync(entitlementId, cancellationToken);
        return entitlement is null || !string.Equals(entitlement.CustomerId, customerId, StringComparison.Ordinal)
            ? throw new UnauthorizedAccessException()
            : entitlement;
    }

    public async Task<IReadOnlyList<EntitlementModel>> GetForCustomerAsync(string customerId, string requestingCustomerId,
        CancellationToken cancellationToken)
    {
        var entitlements = await entitlementService.GetForCustomerAsync(customerId, cancellationToken);
        if (string.Equals(customerId, requestingCustomerId, StringComparison.Ordinal))
        {
            return entitlements;
        }

        var authorizedEntitlements = new List<EntitlementModel>(entitlements.Count);
        foreach (var entitlement in entitlements)
        {
            if (await organizationAuthorizationService.CanViewOtherCustomersBookingsAsync(
                    entitlement.OrganizationId,
                    requestingCustomerId,
                    cancellationToken))
            {
                authorizedEntitlements.Add(entitlement);
            }
        }

        return authorizedEntitlements;
    }

    public async Task<IReadOnlyList<EntitlementModel>> GetForOrganizationAsync(string organizationId, string customerId,
        CancellationToken cancellationToken)
    {
        if (!await organizationAuthorizationService.CanOverrideCancellationPolicyAsync(organizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return await entitlementService.GetForOrganizationAsync(organizationId, cancellationToken);
    }

    public async Task<EntitlementModel> GetAuthorizedForAdjustmentAsync(string entitlementId, string customerId,
        CancellationToken cancellationToken)
    {
        var entitlement = await entitlementService.GetByIdAsync(entitlementId, cancellationToken)
                          ?? throw new KeyNotFoundException($"Entitlement '{entitlementId}' was not found.");
        if (!await organizationAuthorizationService.CanOverrideCancellationPolicyAsync(
                entitlement.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return entitlement;
    }

    public async Task<EntitlementModel> GetAuthorizedForCancellationAsync(string entitlementId, string customerId,
        CancellationToken cancellationToken)
    {
        var entitlement = await entitlementService.GetByIdAsync(entitlementId, cancellationToken)
                          ?? throw new KeyNotFoundException($"Entitlement '{entitlementId}' was not found.");
        if (string.Equals(entitlement.CustomerId, customerId, StringComparison.Ordinal) ||
            await organizationAuthorizationService.CanOverrideCancellationPolicyAsync(
                entitlement.OrganizationId, customerId, cancellationToken))
        {
            return entitlement;
        }

        throw new UnauthorizedAccessException();
    }

    public Task<EntitlementModel> GetAuthorizedForRenewalAsync(string entitlementId, string customerId,
        CancellationToken cancellationToken) => GetAuthorizedForCancellationAsync(entitlementId, customerId, cancellationToken);
}
