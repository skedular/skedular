using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.Mappers;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;

namespace Booking.Api.Services;

public interface IResourceService
{
    Task<ICollection<Resource>> GetAvailableResourcesAsync(
        string? organizationId,
        string? organizationCustomDomain,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> customTagIds,
        ICollection<string> zoneIds,
        ICollection<string> resourceIdsToInclude,
        string? productId,
        CancellationToken cancellationToken);

    Task<int> GetAvailableResourcesCountAsync(
        string? organizationId,
        string? organizationCustomDomain,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> customTagIds,
        ICollection<string> zoneIds,
        string? productId,
        CancellationToken cancellationToken);

    Task<(int, int)> GetOrganizationResourceAvailabilityAsync(
        string? organizationId,
        string? organizationCustomDomain,
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
        string? organizationCustomDomain,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> customTagIds,
        ICollection<string> zoneIds,
        ICollection<string> resourceIdsToInclude,
        string? productId,
        CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               organizationId,
                               organizationCustomDomain,
                               false,
                               false,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        if (organization.Type == OrganizationTypeConstants.Private)
        {
            var customer = await cachedCustomerService.GetAsync(cancellationToken);

            if (!await organizationAuthorizationService.CanViewOrganizationDetailsAsync(organization.Id, customer.Id, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        ICollection<string> productRelatedTags = [];
        if (!string.IsNullOrWhiteSpace(productId))
        {
            var product = await repositoryFactory.ProductRepository.GetByIdAsync(productId, cancellationToken) ?? throw new ProductNotFound();
            if (product.OrganizationId != organization.Id)
            {
                throw new ProductOrganizationDidNotMatch();
            }

            var productVersion = product.ProductVersions.OrderByDescending(item => item.CreatedAt).First();
            productRelatedTags = productVersion.OrganizationTags
                .Where(item => item.Type == OrganizationTagTypeConstants.Product)
                .Select(item => item.Id)
                .ToList();
        }

        var resources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
            organization.Id,
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
        string? organizationId,
        string? organizationCustomDomain,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               organizationId,
                               organizationCustomDomain,
                               false,
                               false,
                               cancellationToken) ??
                           throw new OrganizationNotFound();

        if (!await organizationAuthorizationService.CanViewOrganizationDetailsAsync(organization.Id, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var locations = await repositoryFactory.LocationRepository.GetByOrganizationIdAsync(organization.Id, false, cancellationToken);
        var resourceCount = locations.Aggregate(0, (acc, item) => item.Resources.Count + acc);
        var availableResourceCount = 0;

        foreach (var location in locations)
        {
            var resources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
                organization.Id,
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

    public async Task<int> GetAvailableResourcesCountAsync(
        string? organizationId,
        string? organizationCustomDomain,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> customTagIds,
        ICollection<string> zoneIds,
        string? productId,
        CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               organizationId,
                               organizationCustomDomain,
                               false,
                               false,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        if (organization.Type == OrganizationTypeConstants.Private)
        {
            var customer = await cachedCustomerService.GetAsync(cancellationToken);

            if (!await organizationAuthorizationService.CanViewOrganizationDetailsAsync(organization.Id, customer.Id, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        ICollection<string> productRelatedTags = [];
        if (!string.IsNullOrWhiteSpace(productId))
        {
            var product = await repositoryFactory.ProductRepository.GetByIdAsync(productId, cancellationToken) ?? throw new ProductNotFound();
            if (product.OrganizationId != organization.Id)
            {
                throw new ProductOrganizationDidNotMatch();
            }

            var productVersion = product.ProductVersions.OrderByDescending(item => item.CreatedAt).First();
            productRelatedTags = productVersion.OrganizationTags
                .Where(item => item.Type == OrganizationTagTypeConstants.Product)
                .Select(item => item.Id)
                .ToList();
        }

        return await repositoryFactory.ResourceRepository.GetAvailableResourcesCountAsync(
            organization.Id,
            locationId,
            from,
            until,
            [],
            customTagIds.Concat(zoneIds).Concat(productRelatedTags).ToList(),
            [],
            cancellationToken);
    }
}
