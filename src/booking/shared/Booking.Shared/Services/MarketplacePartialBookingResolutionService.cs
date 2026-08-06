using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Services;

public interface IMarketplacePartialBookingResolutionService
{
    Task<MarketplaceBookingFailureSummary> ResolveAsync(string failureId, string decision, string? actorCustomerId,
        CancellationToken cancellationToken);
}

/// <summary>Single idempotent boundary for customer decisions and expiry of a partial marketplace booking.</summary>
public sealed class MarketplacePartialBookingResolutionService(
    IRepositoryFactory repositoryFactory,
    IDbTransactionBuilder transactionBuilder,
    IMarketplaceRefundOwnershipService refundOwnershipService,
    IMarketplaceRefundService marketplaceRefundService,
    ITemporalOutboxService temporalOutboxService,
    IRandomHelper randomHelper,
    TimeProvider timeProvider) : IMarketplacePartialBookingResolutionService
{
    public async Task<MarketplaceBookingFailureSummary> ResolveAsync(string failureId, string decision, string? actorCustomerId,
        CancellationToken cancellationToken)
    {
        if (decision is not (MarketplaceBookingFailureResolutionDecisionConstants.Accepted
            or MarketplaceBookingFailureResolutionDecisionConstants.Declined or MarketplaceBookingFailureResolutionDecisionConstants.Expired))
        {
            throw new ArgumentOutOfRangeException(nameof(decision));
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
        var failure = await repositoryFactory.MarketplaceBookingFailureRepository.GetByIdAsync(failureId, cancellationToken)
                      ?? throw new InvalidOperationException($"Marketplace booking failure was not found: {failureId}");
        if (failure.ResolutionDecision is not null)
        {
            await transaction.CommitAsync(cancellationToken);
            return failure;
        }

        if (actorCustomerId is not null && !failure.Deliveries.Any(item => item.RecipientCustomerId == actorCustomerId))
        {
            throw new UnauthorizedAccessException();
        }

        if (failure.ResolutionDeadlineAt <= timeProvider.GetUtcNow() && decision == MarketplaceBookingFailureResolutionDecisionConstants.Accepted)
        {
            decision = MarketplaceBookingFailureResolutionDecisionConstants.Expired;
        }

        var ownership = refundOwnershipService.Resolve(failure);

        MarketplaceRefund? refundToAutomate = null;
        if (decision == MarketplaceBookingFailureResolutionDecisionConstants.Accepted &&
            ownership.Scope == MarketplaceRefundOwnershipScope.OneTimeBooking && failure.AllocatedRefundAmount is > 0)
        {
            var booking = await repositoryFactory.BookingRepository.GetByIdAsync(ownership.BookingId!, cancellationToken);
            var total = booking?.MarketplaceBooking?.TotalAmount;
            if (booking is not null && total is not null && total >= failure.AllocatedRefundAmount)
            {
                refundToAutomate = await marketplaceRefundService.CreateModificationRefundAsync(
                    booking, total.Value, total.Value - failure.AllocatedRefundAmount.Value, null, cancellationToken);
            }
        }

        if (decision is MarketplaceBookingFailureResolutionDecisionConstants.Declined or MarketplaceBookingFailureResolutionDecisionConstants.Expired)
        {
            var created = await repositoryFactory.BookingRepository.GetByIdsMinimalAsync(failure.CreatedOccurrenceIds.ToList(), cancellationToken);
            foreach (var occurrence in created)
            {
                repositoryFactory.BookingRepository.Remove(occurrence);
            }

            if (ownership.Scope == MarketplaceRefundOwnershipScope.OneTimeBooking)
            {
                var booking = await repositoryFactory.BookingRepository.GetByIdAsync(ownership.BookingId!, cancellationToken);
                if (booking is not null)
                {
                    refundToAutomate = await marketplaceRefundService.CreateBookingCancellationRefundAsync(booking, null, cancellationToken, true);
                }
            }
            else if (ownership.Scope == MarketplaceRefundOwnershipScope.SubscriptionBillingWindow)
            {
                var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(
                    ownership.MarketplaceBookingSubscriptionId!, cancellationToken);
                if (subscription is not null)
                {
                    refundToAutomate = await marketplaceRefundService.CreateImmediateSubscriptionCancellationRefundAsync(
                        subscription, null, cancellationToken, true);
                }
            }
        }

        failure.ResolutionDecision = decision;
        failure.ResolutionDecidedAt = timeProvider.GetUtcNow();
        failure.ResolutionActorCustomerId = actorCustomerId;
        repositoryFactory.MarketplaceBookingFailureEventRepository.Add(new MarketplaceBookingFailureEvent
        {
            Id = randomHelper.Generate(),
            MarketplaceBookingFailureId = failure.Id,
            ActorCustomerId = actorCustomerId,
            OccurredAt = failure.ResolutionDecidedAt.Value,
            EventType = decision == MarketplaceBookingFailureResolutionDecisionConstants.Accepted
                ? MarketplaceBookingFailureEventTypeConstants.ResolutionAccepted
                : decision == MarketplaceBookingFailureResolutionDecisionConstants.Declined
                    ? MarketplaceBookingFailureEventTypeConstants.ResolutionDeclined
                    : MarketplaceBookingFailureEventTypeConstants.ResolutionExpired,
        });
        repositoryFactory.MarketplaceBookingFailureRepository.Update(failure);
        try
        {
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            if (refundToAutomate is not null)
            {
                temporalOutboxService.StartWorkflowProcessMarketplaceRefund(
                    new ProcessMarketplaceRefundInput(refundToAutomate.Id, actorCustomerId), repositoryFactory.UnitOfWork);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            repositoryFactory.ResetChangeTracker();
            var winner = await repositoryFactory.MarketplaceBookingFailureRepository.GetByIdAsync(failureId, cancellationToken)
                         ?? throw new InvalidOperationException(
                             $"Marketplace booking failure was not found after a concurrent resolution: {failureId}");
            if (winner.ResolutionDecision is not null)
            {
                return winner;
            }

            throw;
        }

        return failure;
    }
}
