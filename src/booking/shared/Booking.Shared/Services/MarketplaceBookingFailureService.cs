using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MarketplaceBookingFailure = Booking.Shared.Database.Entities.MarketplaceBookingFailure;

namespace Booking.Shared.Services;

public interface IMarketplaceBookingFailureService
{
    Task<MarketplaceBookingFailureSummary> FinalizeAsync(
        MarketplaceBookingFailureFinalization finalization,
        CancellationToken cancellationToken);
}

public class MarketplaceBookingFailureService(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ITemporalOutboxService temporalOutboxService,
    IMarketplaceBookingFailureNotificationService notificationService,
    ILogger<MarketplaceBookingFailureService> logger) : IMarketplaceBookingFailureService
{
    public async Task<MarketplaceBookingFailureSummary> FinalizeAsync(
        MarketplaceBookingFailureFinalization finalization,
        CancellationToken cancellationToken)
    {
        var failureKey = MarketplaceBookingFailureKey.Create(finalization);
        var failure = await repositoryFactory.MarketplaceBookingFailureRepository
            .GetByFailureKeyAsync(failureKey, cancellationToken);
        if (failure is not null)
        {
            logger.LogInformation(
                "Marketplace booking failure was already finalized. FailureKey={FailureKey}, FailureId={FailureId}, Category={Category}, Scope={Scope}, CorrelationId={CorrelationId}",
                failureKey,
                failure.Id,
                failure.Category,
                failure.Scope,
                finalization.CorrelationId);
            return ToModel(failure);
        }

        failure = repositoryFactory.MarketplaceBookingFailureRepository.Add(new MarketplaceBookingFailure
        {
            Id = randomHelper.Generate(),
            FailureKey = failureKey,
            Category = finalization.Category,
            Scope = finalization.Scope,
            FinalizedAt = finalization.FinalizedAt,
            BookingId = finalization.BookingId,
            RecurringBookingId = finalization.RecurringBookingId,
            MarketplaceBookingSubscriptionId = finalization.MarketplaceBookingSubscriptionId,
            RequestedFrom = finalization.RequestedFrom,
            RequestedUntil = finalization.RequestedUntil,
            RequestedResourceIds = finalization.RequestedResourceIds.Distinct().ToList(),
            CustomerAction = finalization.CustomerAction,
            CorrelationId = finalization.CorrelationId,
            Reason = finalization.Reason
        });

        repositoryFactory.MarketplaceBookingFailureEventRepository.Add(new MarketplaceBookingFailureEvent
        {
            Id = randomHelper.Generate(),
            MarketplaceBookingFailureId = failure.Id,
            EventType = MarketplaceBookingFailureEventTypeConstants.Finalized,
            OccurredAt = finalization.FinalizedAt,
            Reason = finalization.Reason,
            ActorCustomerId = finalization.ActorCustomerId
        });

        var recipients = finalization.Recipients.Count == 0
            ? await notificationService.ResolveRecipientsAsync(failure, cancellationToken)
            : finalization.Recipients;

        foreach (var recipient in recipients
                     .GroupBy(item => $"{item.RecipientKey}\u001f{item.Channel}", StringComparer.OrdinalIgnoreCase)
                     .Select(item => item.First()))
        {
            repositoryFactory.MarketplaceBookingFailureDeliveryRepository.Add(new MarketplaceBookingFailureDelivery
            {
                Id = randomHelper.Generate(),
                MarketplaceBookingFailureId = failure.Id,
                RecipientKey = recipient.RecipientKey,
                RecipientCustomerId = recipient.RecipientCustomerId,
                RecipientEmail = recipient.RecipientEmail,
                Audience = recipient.Audience,
                Channel = recipient.Channel,
                Status = MarketplaceBookingFailureDeliveryStatusConstants.Pending
            });
        }

        repositoryFactory.MarketplaceBookingFailureEventRepository.Add(new MarketplaceBookingFailureEvent
        {
            Id = randomHelper.Generate(),
            MarketplaceBookingFailureId = failure.Id,
            EventType = MarketplaceBookingFailureEventTypeConstants.DispatchQueued,
            OccurredAt = finalization.FinalizedAt
        });

        temporalOutboxService.StartWorkflowNotifyMarketplaceBookingFailure(
            new NotifyMarketplaceBookingFailureInput(failure.Id),
            repositoryFactory.UnitOfWork);

        try
        {
            // Persist the unique key here so concurrent finalizers can resolve the winner
            // instead of propagating a uniqueness violation from a later unrelated save.
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsFailureKeyConflict(exception))
        {
            // PostgreSQL aborts an explicit transaction after a unique violation. The
            // transaction owner must roll it back before a competing failure can be read.
            // Re-throw here so the surrounding activity/workflow retries on a clean scope.
            if (repositoryFactory.UnitOfWork.HasActiveTransaction)
            {
                throw;
            }

            repositoryFactory.ResetChangeTracker();
            var existingFailure = await repositoryFactory.MarketplaceBookingFailureRepository
                                      .GetByFailureKeyAsync(failureKey, cancellationToken)
                                  ?? throw new InvalidOperationException(
                                      $"Marketplace booking failure key conflict could not be reloaded: {failureKey}");
            return ToModel(existingFailure);
        }

        logger.LogInformation(
            "Finalized marketplace booking failure. FailureId={FailureId}, FailureKey={FailureKey}, BookingId={BookingId}, RecurringBookingId={RecurringBookingId}, SubscriptionId={SubscriptionId}, Category={Category}, Scope={Scope}, CorrelationId={CorrelationId}",
            failure.Id,
            failure.FailureKey,
            failure.BookingId,
            failure.RecurringBookingId,
            failure.MarketplaceBookingSubscriptionId,
            failure.Category,
            failure.Scope,
            failure.CorrelationId);

        return ToModel(failure);
    }

    private static MarketplaceBookingFailureSummary ToModel(MarketplaceBookingFailure failure) =>
        new(failure.Id, failure.Category, failure.Scope, failure.FinalizedAt, failure.RequestedFrom, failure.RequestedUntil,
            failure.CustomerAction ?? string.Empty);

    private static bool IsFailureKeyConflict(DbUpdateException exception) =>
        exception.InnerException?.Message.Contains("23505", StringComparison.Ordinal) == true ||
        exception.Message.Contains("23505", StringComparison.Ordinal);
}
