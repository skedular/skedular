using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IResourceTypeService
{
    Task<ResourceType?> GetByIdAsync(string resourceTypeId, CancellationToken cancellationToken);
    Task<ResourceType> AddAsync(ResourceType resourceType, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);
    Task<ResourceType> UpdateAsync(ResourceType resourceType, CancellationToken cancellationToken);
    Task<ResourceType> DeleteAsync(string resourceTypeId, CancellationToken cancellationToken);
    Task<ICollection<ResourceType>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<ResourceType>>, int)> GetPaginatedResourceTypesAsync(
        PaginationInputParam paginationInputParam,
        ResourceTypeSearchCriteria searchCriteria,
        ICollection<ResourceTypeOrder> orderByFields,
        CancellationToken cancellationToken);
}

public class ResourceTypeService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICachedCustomerService cachedCustomerService,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IMapper mapper,
    IOrganizationOutboxPublisher organizationOutboxPublisher) : IResourceTypeService
{
    public async Task<ResourceType?> GetByIdAsync(string resourceTypeId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceTypeId);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var resourceType = await repositoryFactory.ResourceTypeRepository.GetByIdAsync(resourceTypeId, cancellationToken);
        if (resourceType is null)
        {
            throw new OrganizationResourceTypeNotFound();
        }

        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdAsync(resourceType.Organization.Id, cancellationToken);
        if (existingOrganization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanModify(existingOrganization, customer))
        {
            throw new Unauthorized();
        }

        return mapper.MapTo(resourceType);
    }

    public async Task<ResourceType> AddAsync(ResourceType resourceType, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceType.Organization.Id);

        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(resourceType.Id))
        {
            var existingResourceType = await repositoryFactory.ResourceTypeRepository.GetByIdAsync(resourceType.Id, cancellationToken);
            if (existingResourceType is not null)
            {
                return await UpdateInternalAsync(resourceType, existingResourceType, customer, cancellationToken);
            }
        }
        else
        {
            resourceType.Id = randomHelper.Generate();
        }

        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdAsync(resourceType.Organization.Id, cancellationToken);
        if (existingOrganization is null)
        {
            throw new OrganizationNotFound();
        }

        if (customer is not null && !organizationAuthorizationService.CanModify(existingOrganization, customer))
        {
            throw new Unauthorized();
        }

        var matchingResourceTypeFound = await repositoryFactory.ResourceTypeRepository
            .Query(new Specification<Shared.Database.Entities.ResourceType>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    query.Organization.Id == resourceType.Organization.Id &&
                                    EF.Functions.ILike(query.Name, resourceType.Name)
            }).AnyAsync(cancellationToken);
        if (matchingResourceTypeFound)
        {
            throw new ResourceTypeWithSameNameExist();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.OrganizationRepository.UnitOfWork,
            cancellationToken);

        var resourceTypeEntity = mapper.MapTo(resourceType, existingOrganization);
        _ = repositoryFactory.ResourceTypeRepository.Add(resourceTypeEntity);

        await organizationOutboxPublisher.PublishOrganizationAsync(
            [mapper.MapTo(existingOrganization)],
            repositoryFactory.ResourceTypeRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.ResourceTypeRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return resourceType;
    }

    public async Task<ResourceType> UpdateAsync(ResourceType resourceType, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceType.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingResourceType = await repositoryFactory.ResourceTypeRepository.GetByIdAsync(resourceType.Id, cancellationToken);
        if (existingResourceType is null)
        {
            throw new OrganizationResourceTypeNotFound();
        }

        return await UpdateInternalAsync(resourceType, existingResourceType, customer, cancellationToken);
    }

    public async Task<ResourceType> DeleteAsync(string resourceTypeId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceTypeId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var resourceType = await repositoryFactory.ResourceTypeRepository.GetByIdAsync(resourceTypeId, cancellationToken);
        if (resourceType is null)
        {
            throw new OrganizationResourceTypeNotFound();
        }

        if (!string.IsNullOrWhiteSpace(resourceType.SystemType))
        {
            throw new BuiltinOrganizationResourceTypeCannotBeRemoved();    
        }

        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdAsync(resourceType.Organization.Id, cancellationToken);
        if (existingOrganization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanModify(existingOrganization, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.ResourceTypeRepository.UnitOfWork,
            cancellationToken);

        var deletedResourceType = mapper.MapTo(repositoryFactory.ResourceTypeRepository.Remove(resourceType));

        var mappedOrganization = mapper.MapTo(existingOrganization);
        mappedOrganization.ResourceTypes = mappedOrganization.ResourceTypes.Where(item => item.Id != resourceTypeId).ToList();

        await organizationOutboxPublisher.PublishOrganizationAsync(
            [mappedOrganization],
            repositoryFactory.ResourceTypeRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.ResourceTypeRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedResourceType;
    }

    public async Task<ICollection<ResourceType>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var resourceTypes = await repositoryFactory.ResourceTypeRepository.GetByIdsAsync(ids, cancellationToken);
        if (resourceTypes.Any(item => !string.IsNullOrWhiteSpace(item.SystemType)))
        {
            throw new BuiltinOrganizationResourceTypeCannotBeRemoved();    
        }

        var organizationIds = resourceTypes.Select(item => item.Organization.Id).ToList();
        var existingOrganizations = await repositoryFactory.OrganizationRepository.GetByIdsAsync(organizationIds, cancellationToken);

        if (existingOrganizations.Any(existingOrganization => !organizationAuthorizationService.CanModify(existingOrganization, customer)))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.ResourceTypeRepository.UnitOfWork,
            cancellationToken);

        repositoryFactory.ResourceTypeRepository.RemoveRange(resourceTypes);
        var deletedResourceTypes = resourceTypes.Select(mapper.MapTo).ToList();

        var mappedOrganizations = existingOrganizations.Select(mapper.MapTo).ToList();
        foreach (var mappedOrganization in mappedOrganizations)
        {
            mappedOrganization.ResourceTypes = mappedOrganization.ResourceTypes.Where(item => !ids.Contains(item.Id)).ToList();
        }

        await organizationOutboxPublisher.PublishOrganizationAsync(
            mappedOrganizations,
            repositoryFactory.ResourceTypeRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.ResourceTypeRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedResourceTypes;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<ResourceType>>, int)>
        GetPaginatedResourceTypesAsync(
            PaginationInputParam paginationInputParam,
            ResourceTypeSearchCriteria searchCriteria,
            ICollection<ResourceTypeOrder> orderByFields,
            CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(searchCriteria.OrganizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanView(organization, customer))
        {
            throw new Unauthorized();
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.ResourceTypeRepository.GetPaginatedResourceTypesAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        return (paginatedInfo, mapper.MapTo(edges, mapper.MapTo(organization)).ToList(), totalCount);
    }

    private async Task<ResourceType> UpdateInternalAsync(
        ResourceType resourceType,
        Shared.Database.Entities.ResourceType existingResourceType,
        Customer? customer,
        CancellationToken cancellationToken)
    {
        var existingOrganization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(existingResourceType.Organization.Id, cancellationToken);
        if (existingOrganization is null)
        {
            throw new OrganizationNotFound();
        }

        if (customer is not null && !organizationAuthorizationService.CanModify(existingOrganization, customer))
        {
            throw new Unauthorized();
        }

        var resourceTypeId = resourceType.Id;
        var resourceTypeName = resourceType.Name;
        var organizationId = existingResourceType.Organization.Id;
        var matchingResourceTypeFound = await repositoryFactory.ResourceTypeRepository
            .Query(new Specification<Shared.Database.Entities.ResourceType>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    query.Organization.Id == organizationId &&
                                    EF.Functions.ILike(query.Name, resourceTypeName) &&
                                    query.Id != resourceTypeId
            }).AnyAsync(cancellationToken);
        if (matchingResourceTypeFound)
        {
            throw new ResourceTypeWithSameNameExist();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.ResourceTypeRepository.UnitOfWork,
            cancellationToken);

        resourceType = mapper.MapTo(
            repositoryFactory.ResourceTypeRepository.Update(mapper.MergeTo(resourceType, existingResourceType, existingOrganization)));

        await organizationOutboxPublisher.PublishOrganizationAsync(
            [mapper.MapTo(existingOrganization)],
            repositoryFactory.ResourceTypeRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.ResourceTypeRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return resourceType;
    }
}
