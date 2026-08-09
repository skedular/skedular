using Api.Shared.Services.Models;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Constants = Api.Shared.Services.Constants;

namespace Booking.Api.Services;

public interface IWorkaroundService
{
    Task RepublishBookingAsync(string bookingId, CancellationToken cancellationToken);
    Task RepublishAllBookingsAsync(CancellationToken cancellationToken);
    Task GenerateLocationResourcesSlotsAsync(string locationId, CancellationToken cancellationToken);
    Task GenerateAllLocationsResourcesSlotsAsync(CancellationToken cancellationToken);
    Task GenerateOrganizationArrearsInvoicesAsync(string organizationId, CancellationToken cancellationToken);
}

public class WorkaroundService(
    IRepositoryFactory repositoryFactory,
    IEntityMapper entityMapper,
    IBookingPublisher bookingPublisher,
    ITemporalService temporalService) : IWorkaroundService
{
    public async Task RepublishBookingAsync(string bookingId, CancellationToken cancellationToken)
    {
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking is null)
        {
            return;
        }

        await bookingPublisher.PublishBookingsAsync([entityMapper.MapTo(booking)], cancellationToken);
    }

    public async Task RepublishAllBookingsAsync(CancellationToken cancellationToken)
    {
        var bookings = await repositoryFactory.BookingRepository.GetAllUntrackedAsync(cancellationToken);
        await bookingPublisher.PublishBookingsAsync([.. bookings.Select(entityMapper.MapTo)], cancellationToken);
    }

    public async Task GenerateLocationResourcesSlotsAsync(string locationId, CancellationToken cancellationToken)
    {
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, false, cancellationToken);
        if (location is null || location.IsReplicatedDeleted() || (location.Organization != null && location.Organization.IsReplicatedDeleted()))
        {
            return;
        }

        if (location.Organization?.CustomDomain == Constants.SkedularPublicLocationsCustomDomainName)
        {
            return;
        }

        await temporalService.StartWorkflowGenerateLocationResourcesSlotsAsync(
            new GenerateLocationResourcesSlotsInput(location.Id, null),
            cancellationToken);
    }

    public async Task GenerateAllLocationsResourcesSlotsAsync(CancellationToken cancellationToken)
    {
        var locations = await repositoryFactory.LocationRepository.GetAllWithActiveOrganizationAsync(false, false, [], cancellationToken);

        foreach (var location in locations
                     .Where(item => item.Organization == null || item.Organization.IsReplicatedNotDeleted())
                     .Where(item => item.Organization?.CustomDomain != Constants.SkedularPublicLocationsCustomDomainName))
        {
            await temporalService.StartWorkflowGenerateLocationResourcesSlotsAsync(
                new GenerateLocationResourcesSlotsInput(location.Id, null),
                cancellationToken);
        }
    }

    public async Task GenerateOrganizationArrearsInvoicesAsync(string organizationId, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
            organizationId,
            null,
            false,
            false,
            cancellationToken);
        if (organization is null || organization.IsReplicatedDeleted())
        {
            return;
        }

        await temporalService.SignalRunOrganizationArrearsBillingWorkflowRunNowAsync(
            new OrganizationArrearsBillingConfiguration(
                organization.Id,
                organization.BillingCycle.ToOrganizationBillingCycle()),
            cancellationToken);
    }
}
