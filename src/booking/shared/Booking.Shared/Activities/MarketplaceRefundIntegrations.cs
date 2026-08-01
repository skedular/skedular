using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public class MarketplaceRefundIntegrations(
    IRepositoryFactory repositoryFactory,
    IMarketplaceRefundAutomationService automationService,
    IMarketplaceRefundExhaustionService exhaustionService)
{
    private const int MaximumProviderAttempts = 3;

    [Activity]
    public async Task ProcessAsync(ProcessMarketplaceRefundInput input)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var refund = await repositoryFactory.MarketplaceRefundRepository.GetByIdAsync(input.RefundId, cancellationToken)
                     ?? throw new InvalidOperationException($"Marketplace refund was not found: {input.RefundId}");
        var processed = await automationService.ProcessAfterRequestAsync(refund, input.ActorCustomerId, cancellationToken);

        if (processed.Status == MarketplaceRefundStatusConstants.Failed && processed.RetryCount < MaximumProviderAttempts)
        {
            throw new MarketplaceRefundProviderRetryException(
                processed.LastError ?? $"Marketplace refund provider attempt failed: {processed.Id}");
        }
    }

    [Activity]
    public async Task MarkProcessingExhaustedAsync(RefundProcessingExhaustedInput input)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        await exhaustionService.FinalizeAsync(input.RefundId, input.Error, cancellationToken);
    }
}
