using Api.Shared.Services.Models;
using Booking.Shared.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Temporalio.Activities;

namespace Booking.Shared.Workflows.Activities;

public record ReleaseBookingResourcesInput(string BookingId);

public class BookingIntegrations(
    IRepositoryFactory repositoryFactory,
    IDbTransactionBuilder transactionBuilder,
    IBookingResourceSlotsHelperService bookingResourceSlotsHelperService,
    IMapper mapper,
    IBookingOutboxPublisher bookingOutboxPublisher)
{
    [Activity]
    public async Task ReleaseBookingResourcesAsync(ReleaseBookingResourcesInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(args.BookingId, cancellationToken);
        if (booking is null || booking.IsDeleted())
        {
            return;
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        booking.Status = booking.StripeCheckoutSession is null
            ? BookingStatusConstants.PaymentRecordNeverCreated
            : BookingStatusConstants.PaymentExpired;

        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(booking);

        bookingOutboxPublisher.PublishBookings([mapper.MapTo(booking)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
