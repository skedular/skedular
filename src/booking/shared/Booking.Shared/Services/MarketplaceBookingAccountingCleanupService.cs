using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace Booking.Shared.Services;

public interface IMarketplaceBookingAccountingCleanupService
{
    Task ReconcileAsync(CancellationToken cancellationToken);
}

public class MarketplaceBookingAccountingCleanupService(
    IRepositoryFactory repositoryFactory,
    IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
    ILogger<MarketplaceBookingAccountingCleanupService> logger) : IMarketplaceBookingAccountingCleanupService
{
    public async Task ReconcileAsync(CancellationToken cancellationToken)
    {
        var failures = await repositoryFactory.MarketplaceBookingFailureRepository
            .GetAccountingCleanupCandidatesAsync(100, cancellationToken);
        foreach (var failure in failures)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                if (failure.BookingId is not null)
                {
                    var booking = await repositoryFactory.BookingRepository.GetByIdAsync(failure.BookingId, cancellationToken);
                    if (booking is null)
                    {
                        throw new InvalidOperationException($"Booking {failure.BookingId} for marketplace failure {failure.Id} was not found.");
                    }

                    await accountingInvoiceCancellationService.CancelBookingAsync(booking, cancellationToken);
                }
                else if (failure.RecurringBookingId is not null)
                {
                    var recurringBooking =
                        await repositoryFactory.RecurringBookingRepository.GetByIdAsync(failure.RecurringBookingId, cancellationToken);
                    if (recurringBooking is null)
                    {
                        throw new InvalidOperationException(
                            $"Recurring booking {failure.RecurringBookingId} for marketplace failure {failure.Id} was not found.");
                    }

                    await accountingInvoiceCancellationService.CancelRecurringBookingAsync(recurringBooking, cancellationToken);
                }
                else
                {
                    throw new InvalidOperationException($"Marketplace failure {failure.Id} has no accounting cleanup target.");
                }

                failure.AccountingCleanupStatus = MarketplaceBookingFailureAccountingCleanupStatusConstants.NotRequired;
                repositoryFactory.MarketplaceBookingFailureRepository.Update(failure);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Accounting cleanup remains in transition-required state for marketplace failure {FailureId}",
                    failure.Id);
                failure.AccountingCleanupStatus = MarketplaceBookingFailureAccountingCleanupStatusConstants.TransitionRequired;
                repositoryFactory.MarketplaceBookingFailureRepository.Update(failure);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
