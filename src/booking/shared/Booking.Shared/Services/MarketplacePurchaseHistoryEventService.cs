using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace Booking.Shared.Services;

public interface IMarketplacePurchaseHistoryEventService
{
    Task<MarketplacePurchaseHistoryEventModel> AppendAsync(
        MarketplacePurchaseHistoryEventModel eventModel,
        string idempotencyKey,
        CancellationToken cancellationToken);
}

/// <summary>
///     Canonical write seam for backend-owned marketplace lifecycle history.
///     Callers provide the business occurrence time and a stable idempotency key;
///     the repository owns persistence, duplicate replay, and database mappings.
/// </summary>
public sealed class MarketplacePurchaseHistoryEventService(
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider,
    ILogger<MarketplacePurchaseHistoryEventService> logger) : IMarketplacePurchaseHistoryEventService
{
    public async Task<MarketplacePurchaseHistoryEventModel> AppendAsync(
        MarketplacePurchaseHistoryEventModel eventModel,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(eventModel);
        ArgumentException.ThrowIfNullOrWhiteSpace(idempotencyKey);

        var recordedAt = eventModel.RecordedAt == default ? timeProvider.GetUtcNow() : eventModel.RecordedAt;
        var recordedEvent = eventModel with
        {
            RecordedAt = recordedAt,
        };
        var result = await repositoryFactory.MarketplacePurchaseHistoryRepository.AppendEventAsync(
            recordedEvent, idempotencyKey, cancellationToken);

        logger.LogInformation(
            "Recorded marketplace purchase history event {EventType} for source {SourceId} with key {IdempotencyKey}",
            result.Type,
            result.SourceId,
            idempotencyKey);
        return result;
    }
}
