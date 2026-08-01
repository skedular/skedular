using Booking.Shared.Models;
using Booking.Shared.Repositories;

namespace Booking.Shared.Services;

public interface IMarketplaceRefundExhaustionService
{
    Task FinalizeAsync(string refundId, string error, CancellationToken cancellationToken);
}

public sealed class MarketplaceRefundExhaustionService(
    IRepositoryFactory repositoryFactory,
    IMarketplaceRefundTransitionService refundTransitionService) : IMarketplaceRefundExhaustionService
{
    public async Task FinalizeAsync(string refundId, string error, CancellationToken cancellationToken)
    {
        var refund = await repositoryFactory.MarketplaceRefundRepository.GetByIdAsync(refundId, cancellationToken);
        if (refund is null || refund.Status is MarketplaceRefundStatusConstants.Completed
                or MarketplaceRefundStatusConstants.Rejected or MarketplaceRefundStatusConstants.Cancelled)
        {
            return;
        }

        refund.RetryCount = Math.Max(refund.RetryCount, 3);
        await refundTransitionService.TransitionAsync(
            refund,
            MarketplaceRefundStatusConstants.Failed,
            $"Automatic refund processing exhausted after three attempts: {error}",
            null,
            null,
            cancellationToken);
    }
}
