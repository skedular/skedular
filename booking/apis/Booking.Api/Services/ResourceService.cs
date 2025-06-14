using Api.Shared.Services;
using Booking.Api.Mappers;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;

namespace Booking.Api.Services;

public interface IResourceService
{
    Task<ICollection<Resource>> GetAvailableResourcesAsync(
        string organizationId,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> customTagIds,
        ICollection<string> zoneIds,
        ICollection<string> resourceIdsToInclude,
        string? productId,
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
        string? productId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

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
                throw new UnauthorizedAccessException();
            }
        }

        ICollection<string> productRelatedTags = [];
        if (!string.IsNullOrWhiteSpace(productId))
        {
            var product = await repositoryFactory.ProductRepository.GetByIdAsync(productId, cancellationToken);
            if (product is null)
            {
                throw new ProductNotFound();
            }

            var productVersion = product.ProductVersions.OrderByDescending(item => item.CreatedAt).First();
            productRelatedTags = productVersion.ProductTags.Concat(productVersion.LocationTags).Select(item => item.Id).ToList();
        }

        var resources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
            organizationId,
            locationId,
            from,
            until,
            [],
            customTagIds.Concat(zoneIds).Concat(productRelatedTags).ToList(),
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
            throw new UnauthorizedAccessException();
        }

        var locations = await repositoryFactory.LocationRepository.GetByOrganizationIdAsync(organizationId, false, cancellationToken);
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
