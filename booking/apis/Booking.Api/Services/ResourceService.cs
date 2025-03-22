using Booking.Api.Mappers;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Exceptions;

namespace Booking.Api.Services;

public interface IResourceService
{
    Task<ICollection<Resource>> GetAvailableResourcesAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> customTagIds,
        ICollection<string> zoneIds,
        ICollection<string> resourceIdsToInclude,
        CancellationToken cancellationToken);

    Task<(int, int)> GetOrganizationResourceAvailabilityAsync(
        string organizationId,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken);
}

public class ResourceService(
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ILocationAuthorizationService locationAuthorizationService,
    IMapper mapper) : IResourceService
{
    public async Task<ICollection<Resource>> GetAvailableResourcesAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> customTagIds,
        ICollection<string> zoneIds,
        ICollection<string> resourceIdsToInclude,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(organizationId) && string.IsNullOrWhiteSpace(locationId))
        {
            throw new ArgumentException($"Both {nameof(organizationId)} and {nameof(locationId)} cannot be null or empty.");
        }

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(organizationId))
        {
            var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, false, false, cancellationToken);
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
            var location = await repositoryFactory.LocationRepository.GetByIdAndExcludeInactiveDesksRoomsResourcesAsync(
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

        var resources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
            organizationId,
            locationId,
            from,
            until,
            [],
            customTagIds.Concat(zoneIds).ToList(),
            [],
            cancellationToken);

        var resourcesToInclude = await repositoryFactory.ResourceRepository.GetByIdsAsync(
            resourceIdsToInclude.Where(item => resources.All(resource => resource.Id != item)).ToList(),
            false,
            cancellationToken);

        resources = resources.Concat(resourcesToInclude).ToList();
        return mapper.MapTo(resources).Select(item =>
        {
            item.Location = mapper.MapTo(resources.Single(resource => resource.Id == item.Id).Location);

            return item;
        }).ToList();
    }

    public async Task<(int, int)> GetOrganizationResourceAvailabilityAsync(
        string organizationId,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, false, false, cancellationToken);
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

        var resourceCount = locations.Aggregate(0, (acc, item) => item.Resources.Count + acc);
        var availableResourceCount = 0;

        foreach (var location in locations)
        {
            var resources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
                organizationId,
                location.Id,
                from,
                until,
                [],
                [],
                [],
                cancellationToken);
            availableResourceCount += resources.Count;
        }

        return (resourceCount, availableResourceCount);
    }
}
