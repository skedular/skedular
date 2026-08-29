using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Microsoft.Extensions.Logging;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public record EnqueueMarketplaceBookingCleanupInput(
    string? BookingId,
    string? RecurringBookingId,
    string? SubscriptionId = null,
    string FailureCategory = MarketplaceBookingFailureCategoryConstants.PaymentFailed);

public class MarketplaceBookingCleanupIntegrations(
    IRepositoryFactory repositoryFactory,
    BookingIntegrations bookingIntegrations,
    MarketplaceBookingSubscriptionIntegrations subscriptionIntegrations,
    IMarketplaceBookingFailureService marketplaceBookingFailureService,
    ITemporalOutboxService temporalOutboxService,
    TimeProvider timeProvider,
    ILogger<MarketplaceBookingCleanupIntegrations> logger)
{
    [Activity]
    public async Task EnqueueAsync(EnqueueMarketplaceBookingCleanupInput input)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var failure = input.BookingId is not null
            ? await repositoryFactory.MarketplaceBookingFailureRepository.GetByBookingIdAsync(input.BookingId, cancellationToken)
            : input.RecurringBookingId is not null
                ? await repositoryFactory.MarketplaceBookingFailureRepository.GetByRecurringBookingIdAsync(input.RecurringBookingId,
                    cancellationToken)
                : input.SubscriptionId is not null
                    ? await repositoryFactory.MarketplaceBookingFailureRepository.GetByMarketplaceBookingSubscriptionIdAsync(input.SubscriptionId,
                        cancellationToken)
                    : null;
        var failureId = failure?.Id ?? await FinalizeMissingFailureAsync(input, cancellationToken);
        if (failureId is null)
        {
            logger.LogWarning(
                "Skipped marketplace booking cleanup enqueue because no failure or cleanup target was found. BookingId={BookingId}, RecurringBookingId={RecurringBookingId}, SubscriptionId={SubscriptionId}",
                input.BookingId,
                input.RecurringBookingId,
                input.SubscriptionId);
            return;
        }

        temporalOutboxService.StartWorkflowMarketplaceBookingCleanup(
            new MarketplaceBookingCleanupInput(failureId), repositoryFactory.UnitOfWork);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<string?> FinalizeMissingFailureAsync(
        EnqueueMarketplaceBookingCleanupInput input,
        CancellationToken cancellationToken)
    {
        if (input.BookingId is not null)
        {
            var booking = await repositoryFactory.BookingRepository.GetByIdAsync(input.BookingId, cancellationToken);
            if (booking is null || booking.DeletedAt.HasValue || booking.MarketplaceBooking is null)
            {
                return null;
            }

            var failure = await marketplaceBookingFailureService.FinalizeAsync(
                new MarketplaceBookingFailureFinalization(
                    null,
                    input.FailureCategory,
                    MarketplaceBookingFailureScopeConstants.OneTimeBooking,
                    timeProvider.GetUtcNow(),
                    booking.Id,
                    null,
                    null,
                    booking.From,
                    booking.Until,
                    [.. booking.InvolvedResources.Select(item => item.Id)],
                    MarketplaceBookingFailureCustomerActionConstants.Rebook,
                    null,
                    "Payment cleanup could not release booking resources after retries were exhausted.",
                    booking.CreatedByCustomer?.Id,
                    []),
                cancellationToken);
            return failure.Id;
        }

        if (input.RecurringBookingId is not null)
        {
            var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(
                input.RecurringBookingId,
                cancellationToken);
            if (recurringBooking is null)
            {
                return null;
            }

            var failure = await marketplaceBookingFailureService.FinalizeAsync(
                new MarketplaceBookingFailureFinalization(
                    null,
                    input.FailureCategory,
                    MarketplaceBookingFailureScopeConstants.RecurringCycle,
                    timeProvider.GetUtcNow(),
                    null,
                    recurringBooking.Id,
                    recurringBooking.MarketplaceBookingSubscription?.Id,
                    recurringBooking.StartDate,
                    recurringBooking.EndDate,
                    [.. recurringBooking.RequestedResources.Select(item => item.Id)],
                    MarketplaceBookingFailureCustomerActionConstants.ReviewSubscription,
                    null,
                    "Payment cleanup could not release recurring booking resources after retries were exhausted.",
                    recurringBooking.CreatedByCustomer?.Id,
                    []),
                cancellationToken);
            return failure.Id;
        }

        if (input.SubscriptionId is not null)
        {
            var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(
                input.SubscriptionId,
                cancellationToken);
            var recurringBooking = subscription?.RecurringBookings.FirstOrDefault(item => !item.DeletedAt.HasValue);
            if (subscription is null || recurringBooking is null)
            {
                return null;
            }

            var failure = await marketplaceBookingFailureService.FinalizeAsync(
                new MarketplaceBookingFailureFinalization(
                    null,
                    input.FailureCategory,
                    MarketplaceBookingFailureScopeConstants.InitialSeries,
                    timeProvider.GetUtcNow(),
                    null,
                    recurringBooking.Id,
                    subscription.Id,
                    recurringBooking.StartDate,
                    recurringBooking.EndDate,
                    [.. subscription.RequestedResources.Select(item => item.Id)],
                    MarketplaceBookingFailureCustomerActionConstants.None,
                    null,
                    "Subscription cleanup could not release resources after retries were exhausted.",
                    subscription.CreatedByCustomer?.Id,
                    []),
                cancellationToken);
            return failure.Id;
        }

        return null;
    }

    [Activity]
    public async Task CleanupAsync(MarketplaceBookingCleanupInput input)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var failure = await repositoryFactory.MarketplaceBookingFailureRepository.GetByIdAsync(input.FailureId, cancellationToken);
        if (failure is null || failure.ResourceReleaseStatus == MarketplaceBookingFailureResourceReleaseStatusConstants.Released)
        {
            return;
        }

        logger.LogInformation("Running marketplace booking cleanup for failure {FailureId}", input.FailureId);
        if (failure.Category == MarketplaceBookingFailureCategoryConstants.PaymentFailed &&
            failure.Scope == MarketplaceBookingFailureScopeConstants.InitialSeries &&
            failure.MarketplaceBookingSubscriptionId is not null)
        {
            await subscriptionIntegrations.ReleaseMarketplaceBookingSubscriptionResourcesAsync(
                new ReleaseMarketplaceBookingSubscriptionResourcesInput(failure.MarketplaceBookingSubscriptionId));
            await marketplaceBookingFailureService.MarkResourcesReleasedAsync(
                failure.Id,
                MarketplaceBookingFailureAccountingCleanupStatus.NotRequired,
                cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        else if (failure.BookingId is not null)
        {
            await bookingIntegrations.ReleaseBookingResourcesAsync(
                new ReleaseBookingResourcesInput(failure.BookingId, failure.Category));
        }
        else if (failure.RecurringBookingId is not null)
        {
            await subscriptionIntegrations.ReleaseRecurringBookingResourcesAsync(
                new ReleaseRecurringBookingResourcesInput(failure.RecurringBookingId, failure.Category));
        }
        else
        {
            throw new InvalidOperationException($"Marketplace booking failure {input.FailureId} has no cleanup target.");
        }
    }
}
