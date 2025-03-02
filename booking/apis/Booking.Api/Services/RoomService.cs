using Booking.Api.Mappers;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Exceptions;
using ArgumentException = System.ArgumentException;

namespace Booking.Api.Services;

public interface IRoomService
{
    Task<ICollection<Room>> GetAvailableRoomsAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset date,
        ICollection<string> roomIdsToInclude,
        ICollection<string> customTagIds,
        ICollection<string> zoneIds,
        bool combineCustomTagsZones,
        CancellationToken cancellationToken);

    Task<(int, int)> GetOrganizationRoomsAvailabilityAsync(
        string organizationId,
        DateTimeOffset date,
        CancellationToken cancellationToken);
}

public class RoomService(
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ILocationAuthorizationService locationAuthorizationService,
    IMapper mapper) : IRoomService
{
    public async Task<ICollection<Room>> GetAvailableRoomsAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset date,
        ICollection<string> roomIdsToInclude,
        ICollection<string> customTagIds,
        ICollection<string> zoneIds,
        bool combineCustomTagsZones,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(organizationId) && string.IsNullOrWhiteSpace(locationId))
        {
            throw new ArgumentException($"Both {nameof(organizationId)} and {nameof(locationId)} cannot be null or empty.");
        }

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(organizationId))
        {
            var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, false, false, false, cancellationToken);
            if (organization is null)
            {
                throw new OrganizationNotFound();
            }

            if (!organizationAuthorizationService.CanViewOrganizationDetails(organization, customer))
            {
                throw new Unauthorized();
            }
        }

        if (!string.IsNullOrWhiteSpace(locationId))
        {
            var location = await repositoryFactory.LocationRepository.GetByIdAndExcludeDeactivatedDesksAndRoomsAsync(
                locationId,
                false,
                false,
                false,
                cancellationToken);
            if (location is null)
            {
                throw new LocationNotFound();
            }

            if (!locationAuthorizationService.CanViewLocationDetails(location, customer))
            {
                throw new Unauthorized();
            }
        }

        var rooms = await repositoryFactory.RoomRepository.GetAvailableRoomsAsync(
            organizationId,
            locationId,
            date,
            roomIdsToInclude,
            customTagIds,
            zoneIds,
            combineCustomTagsZones,
            cancellationToken);

        return mapper.MapTo(rooms).Select(item =>
        {
            item.Location = mapper.MapTo(rooms.Single(room => room.Id == item.Id).Location);

            return item;
        }).ToList();
    }

    public async Task<(int, int)> GetOrganizationRoomsAvailabilityAsync(
        string organizationId,
        DateTimeOffset date,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, false, false, false, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanViewOrganizationDetails(organization, customer))
        {
            throw new Unauthorized();
        }

        var locations = await repositoryFactory.LocationRepository.GetByOrganizationIdAsync(
            organizationId,
            false,
            false,
            false,
            cancellationToken);

        var roomsCount = locations.Aggregate(0, (acc, item) => item.Rooms.Count + acc);
        var availableRoomsCount = 0;

        foreach (var location in locations)
        {
            var availableRooms = await repositoryFactory.RoomRepository.GetAvailableRoomsAsync(
                organizationId,
                location.Id,
                date,
                [],
                [],
                [],
                false,
                cancellationToken);
            availableRoomsCount += availableRooms.Count;
        }

        return (roomsCount, availableRoomsCount);
    }
}
