using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Location.Api.Mappers;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;
using Resource = Location.Shared.Database.Entities.Resource;

namespace Location.Api.Services;

public interface IFloorPlanService
{
    Task<FloorPlan?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<FloorPlan> AddAsync(FloorPlan floorPlan, bool updateResourcePositions, CancellationToken cancellationToken);
    Task<FloorPlan> UpdateAsync(FloorPlan floorPlan, bool updateResourcePositions, CancellationToken cancellationToken);
    Task<FloorPlan> DeleteAsync(string id, CancellationToken cancellationToken);

    Task<FloorPlan> UpdateResourcePositionsAsync(
        string floorPlanId,
        IReadOnlyList<ResourcePosition> resourcePositions,
        CancellationToken cancellationToken);

    Task<(PaginatedInfo, IReadOnlyList<Edge<FloorPlan>>, int )> GetPaginatedFloorPlansAsync(
        PaginationInputParam paginationInputParam,
        FloorPlanSearchCriteria searchCriteria,
        IReadOnlyList<FloorPlanOrder> orderByFields,
        CancellationToken cancellationToken);
}

public class FloorPlanService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ICachedCustomerService cachedCustomerService,
    IRandomHelper randomHelper,
    IMapper mapper,
    ICachedLocationService cachedLocationService) : IFloorPlanService
{
    public async Task<FloorPlan?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingFloorPlan = await repositoryFactory.FloorPlanRepository.GetByIdAsync(id, cancellationToken) ?? throw new FloorPlanNotFound();
        var existingLocation = await cachedLocationService.GetByIdAsync(existingFloorPlan.Location.Id, cancellationToken) ??
                               throw new LocationNotFound();

        if (!await organizationAuthorizationService.CanViewAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return mapper.MapTo(existingFloorPlan);
    }

    public async Task<FloorPlan> AddAsync(FloorPlan floorPlan, bool updateResourcePositions, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(floorPlan.Location);
        ArgumentException.ThrowIfNullOrWhiteSpace(floorPlan.Location.Id);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingLocation =
            await repositoryFactory.LocationRepository.GetByIdAsync(floorPlan.Location.Id, cancellationToken) ?? throw new LocationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        if (!string.IsNullOrWhiteSpace(floorPlan.Id))
        {
            var existingFloorPlan = await repositoryFactory.FloorPlanRepository.GetByIdAsync(floorPlan.Id, cancellationToken);
            if (existingFloorPlan is not null)
            {
                if (existingFloorPlan.Location.Id != existingLocation.Id)
                {
                    throw new UnauthorizedAccessException();
                }

                return await UpdateInternalAsync(floorPlan, updateResourcePositions, existingFloorPlan, existingLocation, cancellationToken);
            }
        }
        else
        {
            floorPlan.Id = randomHelper.Generate();
            foreach (var resourcePosition in floorPlan.ResourcePositions)
            {
                resourcePosition.FloorPlan.Id = floorPlan.Id;
            }
        }

        var resourcePositions = floorPlan.ResourcePositions;
        IReadOnlyList<Resource> resources = [];
        if (updateResourcePositions)
        {
            resources = resourcePositions.Count == 0
                ? []
                : await repositoryFactory.ResourceRepository.GetByIdsAsync(
                    resourcePositions.Select(item => item.Resource.Id).ToList(),
                    cancellationToken);
            if (resources.Any(item => item.Location.Id != existingLocation.Id))
            {
                throw new ResourceAndFloorPlanLocationMismatch();
            }

            if (resources.Any(item => item.ResourcePosition is not null))
            {
                throw new ResourceIsPlacedOnDifferentFloorPlan();
            }
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var floorPlanEntity = mapper.MapTo(floorPlan, existingLocation, []);
        if (updateResourcePositions)
        {
            floorPlanEntity.ResourcePositions = resourcePositions
                .Select(resourcePosition =>
                {
                    resourcePosition.Id = randomHelper.Generate();

                    return repositoryFactory.ResourcePositionRepository.Add(
                        mapper.MapToEntity(
                            resourcePosition,
                            resources.First(item => item.Id == resourcePosition.Resource.Id),
                            floorPlanEntity));
                }).ToList();
        }

        repositoryFactory.FloorPlanRepository.Add(floorPlanEntity);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return floorPlan;
    }

    public async Task<FloorPlan> UpdateAsync(FloorPlan floorPlan, bool updateResourcePositions, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(floorPlan.Id);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingFloorPlan = await repositoryFactory.FloorPlanRepository.GetByIdAsync(floorPlan.Id, cancellationToken) ??
                                throw new FloorPlanNotFound();

        var existingLocation =
            await repositoryFactory.LocationRepository.GetByIdAsync(existingFloorPlan.Location.Id, cancellationToken) ?? throw new LocationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return await UpdateInternalAsync(floorPlan, updateResourcePositions, existingFloorPlan, existingLocation, cancellationToken);
    }

    public async Task<FloorPlan> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingFloorPlan = await repositoryFactory.FloorPlanRepository.GetByIdAsync(id, cancellationToken) ?? throw new FloorPlanNotFound();
        var existingLocation =
            await repositoryFactory.LocationRepository.GetByIdAsync(existingFloorPlan.Location.Id, cancellationToken) ?? throw new LocationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.ResourcePositionRepository.RemoveRange(existingFloorPlan.ResourcePositions);
        existingFloorPlan.ResourcePositions = [];
        repositoryFactory.FloorPlanRepository.Remove(existingFloorPlan);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(existingFloorPlan);
    }

    public async Task<FloorPlan> UpdateResourcePositionsAsync(
        string floorPlanId,
        IReadOnlyList<ResourcePosition> resourcePositions,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingFloorPlan = await repositoryFactory.FloorPlanRepository.GetByIdAsync(floorPlanId, cancellationToken) ??
                                throw new FloorPlanNotFound();
        var existingLocation =
            await repositoryFactory.LocationRepository.GetByIdAsync(existingFloorPlan.Location.Id, cancellationToken) ?? throw new LocationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var resources = resourcePositions.Count == 0
            ? []
            : await repositoryFactory.ResourceRepository.GetByIdsAsync(
                resourcePositions.Select(item => item.Resource.Id).ToList(),
                cancellationToken);

        if (resources.Any(item => item.Location.Id != existingFloorPlan.Location.Id))
        {
            throw new ResourceAndFloorPlanLocationMismatch();
        }

        if (resources.Any(item => item.ResourcePosition is not null && item.ResourcePosition.FloorPlan.Id != floorPlanId))
        {
            throw new ResourceIsPlacedOnDifferentFloorPlan();
        }

        var resourcePositionToRemove = existingFloorPlan.ResourcePositions
            .Where(resourcePosition => resourcePositions.All(item => item.Resource.Id != resourcePosition.Resource.Id))
            .ToList();
        var updatedResourcePosition = existingFloorPlan.ResourcePositions
            .Where(resourcePosition => resourcePositions.Any(item => item.Resource.Id == resourcePosition.Resource.Id))
            .Select(resourcePosition =>
            {
                var matchingResourcePosition = resourcePositions.First(item => item.Resource.Id == resourcePosition.Resource.Id);
                matchingResourcePosition.Id = resourcePosition.Id;

                return repositoryFactory.ResourcePositionRepository.Update(
                    mapper.MergeToEntity(
                        matchingResourcePosition,
                        resourcePosition,
                        resources.First(item => item.Id == resourcePosition.Resource.Id),
                        existingFloorPlan));
            })
            .ToList();

        var addedResourcePosition = resourcePositions
            .Where(resourcePosition => existingFloorPlan.ResourcePositions.All(item => item.Resource.Id != resourcePosition.Resource.Id))
            .Select(resourcePosition =>
            {
                resourcePosition.Id = randomHelper.Generate();

                return repositoryFactory.ResourcePositionRepository.Add(
                    mapper.MapToEntity(
                        resourcePosition,
                        resources.First(item => item.Id == resourcePosition.Resource.Id),
                        existingFloorPlan));
            })
            .ToList();

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.ResourcePositionRepository.RemoveRange(resourcePositionToRemove);
        existingFloorPlan.ResourcePositions = addedResourcePosition.Concat(updatedResourcePosition).ToList();

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(existingFloorPlan);
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<FloorPlan>>, int)> GetPaginatedFloorPlansAsync(
        PaginationInputParam paginationInputParam,
        FloorPlanSearchCriteria searchCriteria,
        IReadOnlyList<FloorPlanOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchCriteria.LocationId);

        var existingLocation = await cachedLocationService.GetByIdAsync(searchCriteria.LocationId, cancellationToken) ?? throw new LocationNotFound();

        if (existingLocation.Type != LocationTypeConstants.Marketplace)
        {
            var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
            if (!await organizationAuthorizationService.CanViewAsync(existingLocation.OrganizationId, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.FloorPlanRepository.GetPaginatedFloorPlansAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        return (paginatedInfo, edges.Select(item => new Edge<FloorPlan>(mapper.MapTo(item.Node), item.Cursor)).ToList(), totalCount);
    }

    private async Task<FloorPlan> UpdateInternalAsync(
        FloorPlan floorPlan,
        bool updateResourcePositions,
        Shared.Database.Entities.FloorPlan existingFloorPlan,
        Shared.Database.Entities.Location existingLocation,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Shared.Database.Entities.ResourcePosition> resourcePositionToRemove = [];
        IReadOnlyList<Shared.Database.Entities.ResourcePosition> updatedResourcePosition = [];
        IReadOnlyList<Shared.Database.Entities.ResourcePosition> addedResourcePosition = [];

        if (updateResourcePositions)
        {
            var resourcePositions = floorPlan.ResourcePositions;
            var resources = resourcePositions.Count == 0
                ? []
                : await repositoryFactory.ResourceRepository.GetByIdsAsync(
                    resourcePositions.Select(item => item.Resource.Id).ToList(),
                    cancellationToken);

            if (resources.Any(item => item.Location.Id != existingFloorPlan.Location.Id))
            {
                throw new ResourceAndFloorPlanLocationMismatch();
            }

            if (resources.Any(item => item.ResourcePosition is not null && item.ResourcePosition.FloorPlan.Id != floorPlan.Id))
            {
                throw new ResourceIsPlacedOnDifferentFloorPlan();
            }

            resourcePositionToRemove = existingFloorPlan.ResourcePositions
                .Where(resourcePosition => resourcePositions.All(item => item.Resource.Id != resourcePosition.Resource.Id))
                .ToList();
            updatedResourcePosition = existingFloorPlan.ResourcePositions
                .Where(resourcePosition => resourcePositions.Any(item => item.Resource.Id == resourcePosition.Resource.Id))
                .Select(resourcePosition =>
                {
                    var matchingResourcePosition = resourcePositions.First(item => item.Resource.Id == resourcePosition.Resource.Id);
                    matchingResourcePosition.Id = resourcePosition.Id;

                    return repositoryFactory.ResourcePositionRepository.Update(
                        mapper.MergeToEntity(
                            matchingResourcePosition,
                            resourcePosition,
                            resources.First(item => item.Id == resourcePosition.Resource.Id),
                            existingFloorPlan));
                })
                .ToList();

            var copiedExistingFloorPlan = existingFloorPlan;
            addedResourcePosition = resourcePositions
                .Where(resourcePosition => copiedExistingFloorPlan.ResourcePositions.All(item => item.Resource.Id != resourcePosition.Resource.Id))
                .Select(resourcePosition =>
                {
                    resourcePosition.Id = randomHelper.Generate();

                    return repositoryFactory.ResourcePositionRepository.Add(
                        mapper.MapToEntity(
                            resourcePosition,
                            resources.First(item => item.Id == resourcePosition.Resource.Id),
                            copiedExistingFloorPlan));
                })
                .ToList();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (updateResourcePositions)
        {
            repositoryFactory.ResourcePositionRepository.RemoveRange(resourcePositionToRemove);
        }

        existingFloorPlan = mapper.MergeTo(
            floorPlan,
            existingFloorPlan,
            existingLocation,
            updateResourcePositions ? addedResourcePosition.Concat(updatedResourcePosition).ToList() : null);
        repositoryFactory.FloorPlanRepository.Update(existingFloorPlan);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return floorPlan;
    }
}
