using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Microsoft.Extensions.Logging;

namespace Booking.Shared.Services;

public interface IMarketplaceBookingCleanupReconciliationService
{
    Task ReconcileAsync(CancellationToken cancellationToken);
}

public class MarketplaceBookingCleanupReconciliationService(
    IRepositoryFactory repositoryFactory,
    ITemporalOutboxService temporalOutboxService,
    TimeProvider timeProvider,
    ILogger<MarketplaceBookingCleanupReconciliationService> logger) : IMarketplaceBookingCleanupReconciliationService
{
    private const int BatchSize = 100;
    private static readonly TimeSpan LeaseDuration = TimeSpan.FromMinutes(10);

    public async Task ReconcileAsync(CancellationToken cancellationToken)
    {
        var candidates = await repositoryFactory.MarketplaceBookingFailureRepository
            .GetCleanupCandidatesAsync(BatchSize, cancellationToken);
        var workerId = $"marketplace-booking-cleanup:{Environment.MachineName}:{Environment.ProcessId}";
        var enqueued = 0;

        foreach (var failure in candidates)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var now = timeProvider.GetUtcNow();
            if (!await repositoryFactory.MarketplaceBookingFailureRepository.TryClaimCleanupAsync(
                    failure.Id, workerId, now, LeaseDuration, cancellationToken))
            {
                continue;
            }

            try
            {
                temporalOutboxService.StartWorkflowMarketplaceBookingCleanup(
                    new MarketplaceBookingCleanupInput(failure.Id), repositoryFactory.UnitOfWork);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                enqueued++;
            }
            finally
            {
                await repositoryFactory.MarketplaceBookingFailureRepository
                    .ReleaseCleanupLeaseAsync(failure.Id, workerId, cancellationToken);
            }
        }

        logger.LogInformation(
            "Marketplace booking cleanup reconciliation completed. Found {CandidateCount} candidates and enqueued {EnqueuedCount} workflows",
            candidates.Count,
            enqueued);
    }
}
