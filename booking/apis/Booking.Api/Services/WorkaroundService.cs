using Booking.Api.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;

namespace Booking.Api.Services;

public interface IWorkaroundService
{
    Task RepublishBookingAsync(string teamId, CancellationToken cancellationToken);
    Task RepublishAllBookingsAsync(CancellationToken cancellationToken);
}

public class WorkaroundService(IRepositoryFactory repositoryFactory, IMapper mapper, IBookingPublisher bookingPublisher) : IWorkaroundService
{
    public async Task RepublishBookingAsync(string teamId, CancellationToken cancellationToken)
    {
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(teamId, cancellationToken);
        if (booking is null)
        {
            return;
        }

        await bookingPublisher.PublishBookingAsync([mapper.MapTo(booking)], cancellationToken);
    }

    public async Task RepublishAllBookingsAsync(CancellationToken cancellationToken)
    {
        var bookings = await repositoryFactory.BookingRepository.GetAllAsync(cancellationToken);
        await bookingPublisher.PublishBookingAsync(bookings.Select(mapper.MapTo), cancellationToken);
    }
}
