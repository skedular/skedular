using Booking.Api.Mappers;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Exceptions;
using ArgumentException = System.ArgumentException;

namespace Booking.Api.Services;

public interface IDeskService
{
    Task<ICollection<Desk>> GetAvailableDesksAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset date,
        ICollection<string> deskIdsToInclude,
        ICollection<string> customTagIds,
        ICollection<string> zoneIds,
        bool combineCustomTagsZones,
        CancellationToken cancellationToken);

    Task<(int, int)> GetOrganizationDesksAvailabilityAsync(string organizationId, DateTimeOffset date, CancellationToken cancellationToken);
}

public class DeskService(
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ILocationAuthorizationService locationAuthorizationService,
    IMapper mapper) : IDeskService
{
    public async Task<ICollection<Desk>> GetAvailableDesksAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset date,
        ICollection<string> deskIdsToInclude,
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

        var desks = await repositoryFactory.DeskRepository.GetAvailableDesksAsync(
            organizationId,
            locationId,
            date,
            deskIdsToInclude,
            customTagIds,
            zoneIds,
            combineCustomTagsZones,
            cancellationToken);

        return mapper.MapTo(desks).Select(item =>
        {
            item.Location = mapper.MapTo(desks.Single(desk => desk.Id == item.Id).Location);

            return item;
        }).ToList();
    }

    public async Task<(int, int)> GetOrganizationDesksAvailabilityAsync(
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
            false,
            cancellationToken);

        var desksCount = locations.Aggregate(0, (acc, item) => item.Desks.Count + acc);
        var availableDesksCount = 0;

        foreach (var location in locations)
        {
            var availableDesks = await repositoryFactory.DeskRepository.GetAvailableDesksAsync(
                organizationId,
                location.Id,
                date,
                [],
                [],
                [],
                false,
                cancellationToken);
            availableDesksCount += availableDesks.Count;
        }

        return (desksCount, availableDesksCount);
    }
}
