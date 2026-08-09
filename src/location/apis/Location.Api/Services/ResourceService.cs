using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Location.Api.Models;
using Location.Api.Services.Authorization;
using Location.Shared.Mappers;
using Location.Shared.Models;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Location.Shared.Services;
using Location.Shared.Services.Cache;
using Location.Shared.Workflows;

namespace Location.Api.Services;

public interface IResourceService
{
    Task<Resource> AddAsync(Resource resource, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);
    Task<Resource> UpdateAsync(ResourcePatchRequest request, CancellationToken cancellationToken);
    Task<Resource> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Resource>> DeleteAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken);
    Task<IReadOnlyList<Resource>> ActivateAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken);
    Task<IReadOnlyList<Resource>> DeactivateAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken);
    Task<Resource> GetByIdAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);

    Task<(PaginatedInfo, IReadOnlyList<Edge<Resource>>, int)> GetPaginatedResourcesAsync(
        PaginationInputParam paginationInputParam,
        ResourceSearchCriteria searchCriteria,
        IReadOnlyList<ResourceOrder> orderByFields,
        CancellationToken cancellationToken);
}

public class ResourceService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    IEntityMapper entityMapper,
    ILocationOutboxPublisher locationOutboxPublisher,
    ICachedResourceService cachedResourceService,
    ICachedLocationService cachedLocationService,
    ITemporalOutboxService temporalOutboxService,
    ILogger<ResourceService> logger) : IResourceService
{
    public async Task<Resource> AddAsync(Resource resource, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        foreach (var tag in resource.Tags)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tag.Id);
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(resource.Location.Id);

        string? customerId = null;
        if (!ignoreAuthorizationCheck)
        {
            customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        }

        if (string.IsNullOrWhiteSpace(resource.Id))
        {
            resource.Id = randomHelper.Generate();
        }
        else
        {
            var existingResource = await repositoryFactory.ResourceRepository.GetByIdAsync(resource.Id, cancellationToken);
            if (existingResource is not null)
            {
                return await UpdateInternalAsync(resource, existingResource, customerId, cancellationToken);
            }
        }

        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(resource.Location.Id, cancellationToken) ??
                               throw new LocationNotFound();
        EnsureResourceIsUserManaged(existingLocation);
        if (!string.IsNullOrWhiteSpace(customerId) &&
            !await organizationOfferingService.IsMoreInteractionAllowedAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!string.IsNullOrWhiteSpace(customerId) &&
            !await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var matchingResourceFound = await repositoryFactory.ResourceRepository.ExistsActiveWithNameAsync(
            resource.Location.Id,
            resource.Name,
            null,
            cancellationToken);
        if (matchingResourceFound)
        {
            throw new ResourceWithSameNameExist();
        }

        var organizationTags = await repositoryFactory.OrganizationTagRepository.GetActiveByIdsForOrganizationAsync(
            [.. resource.Tags.Select(item => item.Id)],
            existingLocation.Organization.Id,
            null,
            cancellationToken);

        var resourceTypeTag = organizationTags
            .Where(item => !string.IsNullOrWhiteSpace(item.Type))
            .Where(item => OrganizationTagTypeConstants.ResourceTypes.Any(tagType => tagType == item.Type!.ToOrganizationTagType()))
            .ToList();

        switch (resourceTypeTag.Count)
        {
            case 0:
                throw new ResourceTypeRequired();
            case > 1:
                throw new OnlySingleResourceTypeAllowed();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var mappedResource =
            entityMapper.MapTo(repositoryFactory.ResourceRepository.Add(entityMapper.MapTo(resource, existingLocation, organizationTags)));
        locationOutboxPublisher.PublishLocations([entityMapper.MapTo(existingLocation)], repositoryFactory.UnitOfWork);

        temporalOutboxService.StartComputeOrganizationLocationsAndProductsRelationships(
            new ComputeOrganizationLocationsAndProductsRelationshipsInput(existingLocation.Organization.Id),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedResourceService.UpdateByIdAsync(mappedResource.Id, cancellationToken);

        return mappedResource;
    }

    public async Task<Resource> UpdateAsync(ResourcePatchRequest request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Resource.Id);

        var editUnits = string.Join(",", request.FieldsToUpdate);
        logger.LogInformation(
            "Resource patch autosave started. ResourceId: {ResourceId}, EditUnits: {EditUnits}",
            request.Resource.Id,
            editUnits);

        try
        {
            var existingResource = await repositoryFactory.ResourceRepository.GetByIdAsync(request.Resource.Id, cancellationToken) ??
                                   throw new ResourceNotFound();
            var resource = entityMapper.MapTo(existingResource);
            Apply(request, resource);

            var updatedResource = await UpdateAsync(resource, cancellationToken);
            logger.LogInformation(
                "Resource patch autosave completed. ResourceId: {ResourceId}, EditUnits: {EditUnits}",
                updatedResource.Id,
                editUnits);
            return updatedResource;
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogWarning(
                exception,
                "Resource patch autosave rejected by authorization. ResourceId: {ResourceId}, EditUnits: {EditUnits}",
                request.Resource.Id,
                editUnits);
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Resource patch autosave failed. ResourceId: {ResourceId}, EditUnits: {EditUnits}",
                request.Resource.Id,
                editUnits);
            throw;
        }
    }

    public async Task<Resource> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var resource = await repositoryFactory.ResourceRepository.GetByIdAsync(id, cancellationToken) ?? throw new ResourceNotFound();
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(resource.Location.Id, cancellationToken) ??
                               throw new LocationNotFound();
        EnsureResourceIsUserManaged(existingLocation);
        if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var deletedResource = entityMapper.MapTo(repositoryFactory.ResourceRepository.Remove(resource), entityMapper.MapTo(existingLocation));

        var mappedLocation = entityMapper.MapTo(existingLocation);
        mappedLocation.Resources = [.. mappedLocation.Resources.Where(item => item.Id != id)];

        locationOutboxPublisher.PublishLocations([mappedLocation], repositoryFactory.UnitOfWork);

        temporalOutboxService.StartComputeOrganizationLocationsAndProductsRelationships(
            new ComputeOrganizationLocationsAndProductsRelationshipsInput(existingLocation.Organization.Id),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedResourceService.RemoveByIdAsync(deletedResource.Id, cancellationToken);

        return deletedResource;
    }

    public async Task<IReadOnlyList<Resource>> DeleteAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var resources = await repositoryFactory.ResourceRepository.GetByIdsAsync(ids, cancellationToken);
        var locationIds = resources.Select(item => item.Location.Id).ToList();
        var existingLocations = await repositoryFactory.LocationRepository.GetByIdsAsync(locationIds, cancellationToken);
        EnsureResourcesAreUserManaged(existingLocations);

        foreach (var existingLocation in existingLocations)
        {
            if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(existingLocation.OrganizationId, customerId, cancellationToken))
            {
                throw new NoMoreInteractionAllowed();
            }
        }

        foreach (var existingOrganization in existingLocations)
        {
            if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization.OrganizationId, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.ResourceRepository.RemoveRange(resources);

        var deletedResources = resources
            .Select(resource => entityMapper.MapTo(resource, entityMapper.MapTo(existingLocations.Single(item => item.Id == resource.Location.Id))))
            .ToList();

        var mappedLocations = existingLocations.Select(entityMapper.MapTo).ToList();
        foreach (var mappedLocation in mappedLocations)
        {
            mappedLocation.Resources = [.. mappedLocation.Resources.Where(item => !ids.Contains(item.Id))];
        }

        locationOutboxPublisher.PublishLocations(mappedLocations, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        foreach (var deletedResource in deletedResources)
        {
            await cachedResourceService.RemoveByIdAsync(deletedResource.Id, cancellationToken);
        }

        return deletedResources;
    }

    public async Task<IReadOnlyList<Resource>> ActivateAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var resources = await repositoryFactory.ResourceRepository.GetByIdsAsync(ids, cancellationToken);
        var locationIds = resources.Select(item => item.Location.Id).ToList();
        var existingLocations = await repositoryFactory.LocationRepository.GetByIdsAsync(locationIds, cancellationToken);
        EnsureResourcesAreUserManaged(existingLocations);

        foreach (var existingLocation in existingLocations)
        {
            if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(existingLocation.OrganizationId, customerId, cancellationToken))
            {
                throw new NoMoreInteractionAllowed();
            }
        }

        foreach (var existingOrganization in existingLocations)
        {
            if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization.OrganizationId, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        foreach (var resource in resources)
        {
            resource.Inactive = false;
            repositoryFactory.ResourceRepository.Update(resource);
        }

        var updatedResources = resources
            .Select(resource => entityMapper.MapTo(resource, entityMapper.MapTo(existingLocations.Single(item => item.Id == resource.Location.Id))))
            .ToList();

        var mappedLocations = existingLocations.Select(entityMapper.MapTo).ToList();
        foreach (var resource in mappedLocations.SelectMany(mappedLocation => mappedLocation.Resources.Where(item => ids.Contains(item.Id))))
        {
            resource.Inactive = false;
        }

        locationOutboxPublisher.PublishLocations(mappedLocations, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        foreach (var updatedResource in updatedResources)
        {
            await cachedResourceService.UpdateByIdAsync(updatedResource.Id, cancellationToken);
        }

        return updatedResources;
    }

    public async Task<IReadOnlyList<Resource>> DeactivateAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var resources = await repositoryFactory.ResourceRepository.GetByIdsAsync(ids, cancellationToken);
        var locationIds = resources.Select(item => item.Location.Id).ToList();
        var existingLocations = await repositoryFactory.LocationRepository.GetByIdsAsync(locationIds, cancellationToken);
        EnsureResourcesAreUserManaged(existingLocations);

        foreach (var existingLocation in existingLocations)
        {
            if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(existingLocation.OrganizationId, customerId, cancellationToken))
            {
                throw new NoMoreInteractionAllowed();
            }
        }

        foreach (var existingOrganization in existingLocations)
        {
            if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization.OrganizationId, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        foreach (var resource in resources)
        {
            resource.Inactive = true;
            repositoryFactory.ResourceRepository.Update(resource);
        }

        var updatedResources = resources
            .Select(resource => entityMapper.MapTo(resource, entityMapper.MapTo(existingLocations.Single(item => item.Id == resource.Location.Id))))
            .ToList();

        var mappedLocations = existingLocations.Select(entityMapper.MapTo).ToList();
        foreach (var resource in mappedLocations.SelectMany(mappedLocation => mappedLocation.Resources.Where(item => ids.Contains(item.Id))))
        {
            resource.Inactive = true;
        }

        locationOutboxPublisher.PublishLocations(mappedLocations, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        foreach (var updatedResource in updatedResources)
        {
            await cachedResourceService.UpdateByIdAsync(updatedResource.Id, cancellationToken);
        }

        return updatedResources;
    }

    public async Task<Resource> GetByIdAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        string? customerId = null;
        if (!ignoreAuthorizationCheck)
        {
            customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        }

        var resource = await cachedResourceService.GetByIdAsync(id, cancellationToken) ?? throw new ResourceNotFound();
        var existingLocation = await cachedLocationService.GetByIdAsync(resource.Location.Id, cancellationToken) ?? throw new LocationNotFound();
        if (!ignoreAuthorizationCheck &&
            !await organizationOfferingService.IsMoreInteractionAllowedAsync(existingLocation.OrganizationId, customerId!, cancellationToken))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!ignoreAuthorizationCheck &&
            !await organizationAuthorizationService.CanViewAsync(existingLocation.Organization.Id, customerId!, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return entityMapper.MapTo(resource);
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<Resource>>, int)> GetPaginatedResourcesAsync(
        PaginationInputParam paginationInputParam,
        ResourceSearchCriteria searchCriteria,
        IReadOnlyList<ResourceOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var existingLocation = await cachedLocationService.GetByIdAsync(searchCriteria.LocationId, cancellationToken) ?? throw new LocationNotFound();

        if (existingLocation.Type != LocationTypeConstants.Marketplace)
        {
            var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
            if (!await organizationAuthorizationService.CanViewAsync(existingLocation.OrganizationId, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.ResourceRepository.GetPaginatedResourcesAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        return (paginatedInfo, [.. entityMapper.MapTo(edges, entityMapper.MapTo(existingLocation))], totalCount);
    }

    private async Task<Resource> UpdateAsync(Resource resource, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resource.Id);

        foreach (var tag in resource.Tags)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tag.Id);
        }

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingResource = await repositoryFactory.ResourceRepository.GetByIdAsync(resource.Id, cancellationToken) ??
                               throw new ResourceNotFound();

        return await UpdateInternalAsync(resource, existingResource, customerId, cancellationToken);
    }

    private async Task<Resource> UpdateInternalAsync(
        Resource resource,
        Shared.Database.Entities.Resource existingResource,
        string? customerId,
        CancellationToken cancellationToken)
    {
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(existingResource.Location.Id, cancellationToken) ??
                               throw new LocationNotFound();
        EnsureResourceIsUserManaged(existingLocation);
        if (!string.IsNullOrWhiteSpace(customerId) &&
            !await organizationOfferingService.IsMoreInteractionAllowedAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!string.IsNullOrWhiteSpace(customerId) &&
            !await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var matchingResourceFound = await repositoryFactory.ResourceRepository.ExistsActiveWithNameAsync(
            existingResource.Location.Id,
            resource.Name,
            resource.Id,
            cancellationToken);
        if (matchingResourceFound)
        {
            throw new ResourceWithSameNameExist();
        }

        var organizationTags = await repositoryFactory.OrganizationTagRepository.GetActiveByIdsForOrganizationAsync(
            [.. resource.Tags.Select(item => item.Id)],
            existingLocation.Organization.Id,
            null,
            cancellationToken);

        var resourceTypeTag = organizationTags
            .Where(item => !string.IsNullOrWhiteSpace(item.Type))
            .Where(item => OrganizationTagTypeConstants.ResourceTypes.Any(tagType => tagType == item.Type!.ToOrganizationTagType()))
            .ToList();

        switch (resourceTypeTag.Count)
        {
            case 0:
                throw new ResourceTypeRequired();
            case > 1:
                throw new OnlySingleResourceTypeAllowed();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var originalIsAvailableHoursOverridden = existingResource.IsAvailableHoursOverridden;
        var originalAvailableHours = existingResource.AvailableHours;

        existingResource = entityMapper.MergeTo(resource, existingResource, existingLocation, organizationTags);

        // Restoring original opening hours
        existingResource.IsAvailableHoursOverridden = originalIsAvailableHoursOverridden;
        existingResource.AvailableHours = originalAvailableHours;

        resource = entityMapper.MapTo(repositoryFactory.ResourceRepository.Update(existingResource), entityMapper.MapTo(existingLocation));

        locationOutboxPublisher.PublishLocations([entityMapper.MapTo(existingLocation)], repositoryFactory.UnitOfWork);

        temporalOutboxService.StartComputeOrganizationLocationsAndProductsRelationships(
            new ComputeOrganizationLocationsAndProductsRelationshipsInput(existingLocation.Organization.Id),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedResourceService.UpdateByIdAsync(resource.Id, cancellationToken);

        return resource;
    }

    private static void Apply(ResourcePatchRequest request, Resource resource)
    {
        foreach (var field in request.FieldsToUpdate)
        {
            switch (field)
            {
                case ResourcePatchField.Name:
                    resource.Name = request.Resource.Name;
                    break;
                case ResourcePatchField.Inactive:
                    resource.Inactive = request.Resource.Inactive;
                    break;
                case ResourcePatchField.RequireBookingApproval:
                    resource.RequireBookingApproval = request.Resource.RequireBookingApproval;
                    break;
                case ResourcePatchField.Color:
                    resource.Color = request.Resource.Color;
                    break;
                case ResourcePatchField.Capacity:
                    resource.Capacity = request.Resource.Capacity;
                    break;
                case ResourcePatchField.Tags:
                case ResourcePatchField.ResourceType:
                    resource.Tags = request.Resource.Tags;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(request.FieldsToUpdate), field,
                        $"Unexpected value for {nameof(request.FieldsToUpdate)}: {field}. Update enum mapping or caller input.");
            }
        }
    }

    private static void EnsureResourcesAreUserManaged(IEnumerable<Shared.Database.Entities.Location> locations)
    {
        foreach (var location in locations)
        {
            EnsureResourceIsUserManaged(location);
        }
    }

    private static void EnsureResourceIsUserManaged(Shared.Database.Entities.Location location)
    {
        if (location.Organization.Type == OrganizationTypeConstants.Host)
        {
            throw new InvalidOperationException("Host resources are system managed and cannot be changed directly.");
        }
    }
}
