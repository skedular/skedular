using Booking.Shared.Mappers;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;

namespace Booking.Shared.Services.Entitlements;

public interface IEntitlementEligibilityService
{
    Task<EntitlementModel?> SelectAsync(
        string customerId,
        string pricingId,
        DateTimeOffset bookingAt,
        CancellationToken cancellationToken);
}

public sealed class EntitlementEligibilityService(
    IRepositoryFactory repositoryFactory,
    ICreditLedgerService creditLedgerService,
    IEntitlementModelMapper entitlementModelMapper)
    : IEntitlementEligibilityService
{
    public async Task<EntitlementModel?> SelectAsync(
        string customerId,
        string pricingId,
        DateTimeOffset bookingAt,
        CancellationToken cancellationToken)
    {
        var entitlements = await repositoryFactory.EntitlementRepository.GetActiveForCustomerAsync(customerId, bookingAt, cancellationToken);
        var entitlement = entitlements.FirstOrDefault(item =>
            item.PricingId == pricingId && creditLedgerService.GetAvailableCredits(item) > 0);
        return entitlement is null ? null : entitlementModelMapper.Map(entitlement);
    }
}
