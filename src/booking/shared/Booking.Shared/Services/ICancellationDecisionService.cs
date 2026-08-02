using Booking.Shared.Models;

namespace Booking.Shared.Services;

public interface ICancellationDecisionService
{
    CancellationDecision ResolveCustomerDecision(
        string customerId,
        string productOwnerOrganizationId,
        bool canManageProduct,
        string? overrideReason);
}

public sealed class CancellationDecisionService : ICancellationDecisionService
{
    public CancellationDecision ResolveCustomerDecision(
        string customerId,
        string productOwnerOrganizationId,
        bool canManageProduct,
        string? overrideReason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(productOwnerOrganizationId);

        if (!canManageProduct && !string.IsNullOrWhiteSpace(overrideReason))
        {
            throw new UnauthorizedAccessException();
        }

        return new CancellationDecision(
            new CancellationActor(
                canManageProduct ? CancellationActorCategory.Administrator : CancellationActorCategory.Customer,
                customerId,
                canManageProduct ? productOwnerOrganizationId : null),
            canManageProduct,
            canManageProduct ? overrideReason : null);
    }
}
