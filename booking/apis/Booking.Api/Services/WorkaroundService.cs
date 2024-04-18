using Booking.Api.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;

namespace Booking.Api.Services;

public interface IWorkaroundService
{
    Task RepublishBookingAsync(string teamId, CancellationToken cancellationToken);
    Task RepublishAllBookingsAsync(CancellationToken cancellationToken);
}

public class WorkaroundService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IBookingPublisher bookingPublisher) : IWorkaroundService
{
    public async Task RepublishBookingAsync(string teamId, CancellationToken cancellationToken)
    {
        var team =
            await repositoryFactory.BookingRepository.GetByIdAsync(teamId, cancellationToken);
        if (team is null)
        {
            return;
        }

        await bookingPublisher.PublishBookingAsync([mapper.MapTo(team)!], cancellationToken);
    }

    public async Task RepublishAllBookingsAsync(CancellationToken cancellationToken)
    {
        var teams = await repositoryFactory.BookingRepository.GetAllAsync(cancellationToken);
        await bookingPublisher.PublishBookingAsync(teams.Select(mapper.MapTo), cancellationToken);
    }
}
