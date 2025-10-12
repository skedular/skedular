using Api.Shared.Services;
using Booking.Api.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows.ResourcesSlots;
using Enterprise.Shared.Database;

namespace Booking.Api.Services;

public interface IWorkaroundService
{
    Task RepublishBookingAsync(string bookingId, CancellationToken cancellationToken);
    Task RepublishAllBookingsAsync(CancellationToken cancellationToken);
    Task GenerateLocationResourcesSlotsAsync(string locationId, CancellationToken cancellationToken);
    Task GenerateAllLocationsResourcesSlotsAsync(CancellationToken cancellationToken);
}

public class WorkaroundService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IBookingPublisher bookingPublisher,
    IBookingCheckoutSessionHelperService bookingCheckoutSessionHelperService,
    ITemporalService temporalService) : IWorkaroundService
{
    public async Task RepublishBookingAsync(string bookingId, CancellationToken cancellationToken)
    {
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking is null)
        {
            return;
        }

        await bookingPublisher.PublishBookingsAsync(
            [mapper.MapTo(booking, bookingCheckoutSessionHelperService.GetBookingPaymentExpiry(booking))],
            cancellationToken);
    }

    public async Task RepublishAllBookingsAsync(CancellationToken cancellationToken)
    {
        var bookings = await repositoryFactory.BookingRepository.GetAllAsync(cancellationToken);
        await bookingPublisher.PublishBookingsAsync(
            bookings.Select(item => mapper.MapTo(item, bookingCheckoutSessionHelperService.GetBookingPaymentExpiry(item))),
            cancellationToken);
    }

    public async Task GenerateLocationResourcesSlotsAsync(string locationId, CancellationToken cancellationToken)
    {
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, false, cancellationToken);
        if (location is null || location.IsReplicatedDeleted() || (location.Organization != null && location.Organization.IsReplicatedDeleted()))
        {
            return;
        }

        if (location.Organization?.UniqueAlphanumericName == Constants.SkedularPublicLocationsUniqueAlphanumericName)
        {
            return;
        }

        await temporalService.StartWorkflowGenerateLocationResourcesSlotsAsync(
            new GenerateLocationResourcesSlotsInput(location.Id, null),
            cancellationToken);
    }

    public async Task GenerateAllLocationsResourcesSlotsAsync(CancellationToken cancellationToken)
    {
        var locations = await repositoryFactory.LocationRepository.GetAllWithActiveOrganizationAsync(false, cancellationToken);

        foreach (var location in locations
                     .Where(item => item.Organization == null || item.Organization.IsReplicatedNotDeleted())
                     .Where(item => item.Organization?.UniqueAlphanumericName != Constants.SkedularPublicLocationsUniqueAlphanumericName))
        {
            await temporalService.StartWorkflowGenerateLocationResourcesSlotsAsync(
                new GenerateLocationResourcesSlotsInput(location.Id, null),
                cancellationToken);
        }
    }
}
