using Booking.Api.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;

namespace Booking.Api.Services;

public interface IWorkaroundService
{
    Task RepublishBookingAsync(string bookingId, CancellationToken cancellationToken);
    Task RepublishAllBookingsAsync(CancellationToken cancellationToken);
}

public class WorkaroundService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IBookingPublisher bookingPublisher,
    IBookingCheckoutSessionHelper bookingCheckoutSessionHelper) : IWorkaroundService
{
    public async Task RepublishBookingAsync(string bookingId, CancellationToken cancellationToken)
    {
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking is null)
        {
            return;
        }

        await bookingPublisher.PublishBookingsAsync([mapper.MapTo(booking, bookingCheckoutSessionHelper.GetBookingCheckoutSessionExpiry(booking))],
            cancellationToken);
    }

    public async Task RepublishAllBookingsAsync(CancellationToken cancellationToken)
    {
        var bookings = await repositoryFactory.BookingRepository.GetAllAsync(cancellationToken);
        await bookingPublisher.PublishBookingsAsync(
            bookings.Select(item => mapper.MapTo(item, bookingCheckoutSessionHelper.GetBookingCheckoutSessionExpiry(item))), cancellationToken);
    }
}
