using Api.Shared.Services.Models;
using Location.Shared.Repositories;
using Microsoft.Extensions.Logging;
using ResourceEntity = Location.Shared.Database.Entities.Resource;

namespace Location.Shared.Services;

public interface IAutoResourceService
{
    Task EnsureForHostLocationAsync(string locationId, string productTagId, CancellationToken cancellationToken);
}

public class AutoResourceService(IRepositoryFactory repositoryFactory, ILogger<AutoResourceService> logger) : IAutoResourceService
{
    public async Task EnsureForHostLocationAsync(string locationId, string productTagId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(productTagId);

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null || location.Organization.Type != OrganizationTypeConstants.Host)
        {
            return;
        }

        var resourceId = HostLocationSystemIds.Resource(location.Id);
        var resourceName = $"Host: {location.Name}";
        var productTag = await repositoryFactory.OrganizationTagRepository.UpsertNakedAsync(
            productTagId,
            location.Organization,
            cancellationToken);
        if (location.OrganizationTags.All(tag => tag.Id != productTag.Id))
        {
            location.OrganizationTags.Add(productTag);
        }

        var existingResource = location.Resources.FirstOrDefault(resource => resource.Id == resourceId);
        if (existingResource is not null)
        {
            var changed = false;
            if (existingResource.DeletedAt.HasValue)
            {
                existingResource.DeletedAt = null;
                changed = true;
            }

            if (existingResource.Name != resourceName)
            {
                existingResource.Name = resourceName;
                changed = true;
            }

            if (existingResource.OrganizationTags.All(current => current.Id != productTag.Id))
            {
                existingResource.OrganizationTags.Add(productTag);
                changed = true;
            }

            if (changed)
            {
                repositoryFactory.ResourceRepository.Update(existingResource);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            }

            logger.LogInformation("Host hidden resource already exists. LocationId: {LocationId}", location.Id);
            return;
        }

        var resourceType = await repositoryFactory.OrganizationTagRepository.GetActiveByTypeForOrganizationAsync(
                               location.OrganizationId,
                               OrganizationTagTypeConstants.ResourceEntireLocation,
                               cancellationToken) ??
                           throw new InvalidOperationException(
                               $"Host organization {location.OrganizationId} does not have the required default resource type tag.");

        var resource = new ResourceEntity
        {
            Id = resourceId,
            Name = resourceName,
            Location = location,
            Capacity = 1,
            Inactive = false,
            RequireBookingApproval = false,
            OrganizationTags = [resourceType, productTag],
        };

        repositoryFactory.ResourceRepository.Add(resource);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Host hidden resource created. OrganizationId: {OrganizationId}, LocationId: {LocationId}, ResourceId: {ResourceId}",
            location.OrganizationId,
            location.Id,
            resource.Id);
    }
}
