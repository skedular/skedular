using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Location.Api.Mappers;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using OrganizationTag = Location.Shared.Database.Entities.OrganizationTag;

namespace Location.Api.Services;

public interface IResourceService
{
    Task<Resource> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Resource> AddAsync(Resource resource, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);
    Task<Resource> UpdateAsync(Resource resource, CancellationToken cancellationToken);
    Task<Resource> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<Resource>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<ICollection<Resource>> ActivateAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<ICollection<Resource>> DeactivateAsync(ICollection<string> ids, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<Resource>>, int)> GetPaginatedResourcesAsync(
        PaginationInputParam paginationInputParam,
        ResourceSearchCriteria searchCriteria,
        ICollection<ResourceOrder> orderByFields,
        CancellationToken cancellationToken);
}

public class ResourceService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICachedCustomerService cachedCustomerService,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    IMapper mapper,
    ILocationOutboxPublisher locationOutboxPublisher) : IResourceService
{
    public async Task<Resource> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var resource = await repositoryFactory.ResourceRepository.GetByIdAsync(id, cancellationToken);
        if (resource is null)
        {
            throw new ResourceNotFound();
        }

        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(resource.Location.Id, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (!organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!organizationAuthorizationService.CanModify(existingLocation.Organization, customer))
        {
            throw new Unauthorized();
        }

        return mapper.MapTo(resource);
    }

    public async Task<Resource> AddAsync(Resource resource, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        foreach (var tag in resource.Tags)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tag.Id);
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(resource.Location.Id);

        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(resource.Id))
        {
            var existingResource = await repositoryFactory.ResourceRepository.GetByIdAsync(resource.Id, cancellationToken);
            if (existingResource is not null)
            {
                return await UpdateInternalAsync(resource, existingResource, customer, cancellationToken);
            }
        }
        else
        {
            resource.Id = randomHelper.Generate();
        }

        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(resource.Location.Id, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (customer is not null && !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (customer is not null && !organizationAuthorizationService.CanModify(existingLocation.Organization, customer))
        {
            throw new Unauthorized();
        }

        var matchingResourceFound = await repositoryFactory.ResourceRepository.Query(
                new Specification<Shared.Database.Entities.Resource>
                {
                    Criteria = query =>
                        !query.DeletedAt.HasValue &&
                        query.Location.Id == resource.Location.Id &&
                        EF.Functions.ILike(query.Name, resource.Name)
                })
            .AnyAsync(cancellationToken);
        if (matchingResourceFound)
        {
            throw new ResourceWithSameNameExist();
        }

        var organizationTags = await repositoryFactory.OrganizationTagRepository.Query(
            new Specification<OrganizationTag>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    resource.Tags.Select(item => item.Id).Contains(query.Id) &&
                                    query.Organization.Id == existingLocation.Organization.Id &&
                                    !query.Organization.DeletedAt.HasValue
            }).ToListAsync(cancellationToken);

        var resourceTypeTag = organizationTags
            .Where(item => !string.IsNullOrWhiteSpace(item.Type))
            .Where(item => OrganizationTagTypeConstants.ResourceTypes.Any(tagType => tagType == item.Type!.ToOrganizationTagType()))
            .ToList();

        if (resourceTypeTag.Count == 0)
        {
            throw new ResourceTypeRequired();
        }

        if (resourceTypeTag.Count > 1)
        {
            throw new OnlySingleResourceTypeAllowed();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var mappedResource = mapper.MapTo(repositoryFactory.ResourceRepository.Add(mapper.MapTo(resource, existingLocation, organizationTags)));
        locationOutboxPublisher.PublishLocations([mapper.MapTo(existingLocation)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return mappedResource;
    }

    public async Task<Resource> UpdateAsync(Resource resource, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resource.Id);

        foreach (var tag in resource.Tags)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tag.Id);
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingResource = await repositoryFactory.ResourceRepository.GetByIdAsync(resource.Id, cancellationToken);
        if (existingResource is null)
        {
            throw new ResourceNotFound();
        }

        return await UpdateInternalAsync(resource, existingResource, customer, cancellationToken);
    }

    public async Task<Resource> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var resource = await repositoryFactory.ResourceRepository.GetByIdAsync(id, cancellationToken);
        if (resource is null)
        {
            throw new ResourceNotFound();
        }

        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(resource.Location.Id, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (!organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!organizationAuthorizationService.CanModify(existingLocation.Organization, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var deletedResource = mapper.MapTo(repositoryFactory.ResourceRepository.Remove(resource), mapper.MapTo(existingLocation));

        var mappedLocation = mapper.MapTo(existingLocation);
        mappedLocation.Resources = mappedLocation.Resources.Where(item => item.Id != id).ToList();

        locationOutboxPublisher.PublishLocations([mappedLocation], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedResource;
    }

    public async Task<ICollection<Resource>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var resources = await repositoryFactory.ResourceRepository.GetByIdsAsync(ids, cancellationToken);
        var locationIds = resources.Select(item => item.Location.Id).ToList();
        var existingLocations = await repositoryFactory.LocationRepository.GetByIdsAsync(locationIds, cancellationToken);

        if (existingLocations.Any(existingLocation => !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer)))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (existingLocations.Any(existingOrganization => !organizationAuthorizationService.CanModify(existingOrganization.Organization, customer)))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.ResourceRepository.RemoveRange(resources);

        var deletedResources = resources
            .Select(resource => mapper.MapTo(resource, mapper.MapTo(existingLocations.Single(item => item.Id == resource.Location.Id))))
            .ToList();

        var mappedLocations = existingLocations.Select(mapper.MapTo).ToList();
        foreach (var mappedLocation in mappedLocations)
        {
            mappedLocation.Resources = mappedLocation.Resources.Where(item => !ids.Contains(item.Id)).ToList();
        }

        locationOutboxPublisher.PublishLocations(mappedLocations, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return deletedResources;
    }

    public async Task<ICollection<Resource>> ActivateAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var resources = await repositoryFactory.ResourceRepository.GetByIdsAsync(ids, cancellationToken);
        var locationIds = resources.Select(item => item.Location.Id).ToList();
        var existingLocations = await repositoryFactory.LocationRepository.GetByIdsAsync(locationIds, cancellationToken);
        if (existingLocations
            .Any(existingLocation => !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer)))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (existingLocations.Any(item => !organizationAuthorizationService.CanModify(item.Organization, customer)))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        foreach (var resource in resources)
        {
            resource.Inactive = false;
            repositoryFactory.ResourceRepository.Update(resource);
        }

        var updatedResources = resources
            .Select(resource => mapper.MapTo(resource, mapper.MapTo(existingLocations.Single(item => item.Id == resource.Location.Id))))
            .ToList();

        var mappedLocations = existingLocations.Select(mapper.MapTo).ToList();
        foreach (var resource in mappedLocations.SelectMany(mappedLocation => mappedLocation.Resources.Where(item => !ids.Contains(item.Id))))
        {
            resource.Inactive = false;
        }

        locationOutboxPublisher.PublishLocations(mappedLocations, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return updatedResources;
    }

    public async Task<ICollection<Resource>> DeactivateAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var resources = await repositoryFactory.ResourceRepository.GetByIdsAsync(ids, cancellationToken);
        var locationIds = resources.Select(item => item.Location.Id).ToList();
        var existingLocations = await repositoryFactory.LocationRepository.GetByIdsAsync(locationIds, cancellationToken);
        if (existingLocations.Any(item => !organizationOfferingService.IsMoreInteractionAllowed(item.Organization, customer)))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (existingLocations.Any(item => !organizationAuthorizationService.CanModify(item.Organization, customer)))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        foreach (var resource in resources)
        {
            resource.Inactive = true;
            repositoryFactory.ResourceRepository.Update(resource);
        }

        var updatedResources = resources
            .Select(resource => mapper.MapTo(resource, mapper.MapTo(existingLocations.Single(item => item.Id == resource.Location.Id))))
            .ToList();

        var mappedLocations = existingLocations.Select(mapper.MapTo).ToList();
        foreach (var resource in mappedLocations.SelectMany(mappedLocation => mappedLocation.Resources.Where(item => !ids.Contains(item.Id))))
        {
            resource.Inactive = true;
        }

        locationOutboxPublisher.PublishLocations(mappedLocations, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return updatedResources;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Resource>>, int)> GetPaginatedResourcesAsync(
        PaginationInputParam paginationInputParam,
        ResourceSearchCriteria searchCriteria,
        ICollection<ResourceOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(searchCriteria.LocationId, cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        if (!organizationAuthorizationService.CanView(location.Organization, customer))
        {
            throw new Unauthorized();
        }

        var (paginatedInfo, edges, totalCount) =
            await repositoryFactory.ResourceRepository.GetPaginatedResourcesAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                cancellationToken);

        return (paginatedInfo, mapper.MapTo(edges, mapper.MapTo(location)).ToList(), totalCount);
    }

    private async Task<Resource> UpdateInternalAsync(
        Resource resource,
        Shared.Database.Entities.Resource existingResource,
        Customer? customer,
        CancellationToken cancellationToken)
    {
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(existingResource.Location.Id, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (customer is not null && !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (customer is not null && !organizationAuthorizationService.CanModify(existingLocation.Organization, customer))
        {
            throw new Unauthorized();
        }

        var resourceId = resource.Id;
        var resourceName = resource.Name;
        var tags = resource.Tags;
        var locationId = existingResource.Location.Id;
        var matchingResourceFound = await repositoryFactory.ResourceRepository.Query(
            new Specification<Shared.Database.Entities.Resource>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    query.Location.Id == locationId &&
                                    EF.Functions.ILike(query.Name, resourceName) &&
                                    query.Id != resourceId
            }).AnyAsync(cancellationToken);
        if (matchingResourceFound)
        {
            throw new ResourceWithSameNameExist();
        }

        var organizationTags = await repositoryFactory.OrganizationTagRepository.Query(
            new Specification<OrganizationTag>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    tags.Select(item => item.Id).Contains(query.Id) &&
                                    query.Organization.Id == existingLocation.Organization.Id &&
                                    !query.Organization.DeletedAt.HasValue
            }).ToListAsync(cancellationToken);

        var resourceTypeTag = organizationTags
            .Where(item => !string.IsNullOrWhiteSpace(item.Type))
            .Where(item => OrganizationTagTypeConstants.ResourceTypes.Any(tagType => tagType == item.Type!.ToOrganizationTagType()))
            .ToList();

        if (resourceTypeTag.Count == 0)
        {
            throw new ResourceTypeRequired();
        }

        if (resourceTypeTag.Count > 1)
        {
            throw new OnlySingleResourceTypeAllowed();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var originalIsAvailableHoursOverridden = existingResource.IsAvailableHoursOverridden;
        var originalAvailableHours = existingResource.AvailableHours;

        existingResource = mapper.MergeTo(resource, existingResource, existingLocation, organizationTags);

        // Restoring original opening hours
        existingResource.IsAvailableHoursOverridden = originalIsAvailableHoursOverridden;
        existingResource.AvailableHours = originalAvailableHours;

        resource = mapper.MapTo(repositoryFactory.ResourceRepository.Update(existingResource), mapper.MapTo(existingLocation));

        locationOutboxPublisher.PublishLocations([mapper.MapTo(existingLocation)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return resource;
    }
}
