using System.Text.Json;
using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Types;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Api.Services.Authorization;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Location.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Path = System.IO.Path;

namespace Location.Api.GraphQL;

[MutationType]
public class Mutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<LocationPayload?> AddLocationAsync(
        AddLocationInput input,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = mapper.MapTo(await locationService.AddAsync(mapper.MapTo(input), false, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<LocationPayload?> UpdateLocationAsync(
        UpdateLocationInput input,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = mapper.MapTo(await locationService.UpdateAsync(mapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<LocationPayload?> DeleteLocationAsync(
        DeleteLocationInput input,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        new() { ClientMutationId = input.ClientMutationId, Location = mapper.MapTo(await locationService.DeleteAsync(input.Id, cancellationToken))! };

    [UseResolverScope]
    public async Task<ResourcePayload?> AddResourceAsync(
        AddResourceInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Resource = mapper.MapTo(await resourceService.AddAsync(mapper.MapTo(input), false, cancellationToken))
        };

    [UseResolverScope]
    public async Task<ResourcePayload?> UpdateResourceAsync(
        UpdateResourceInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Resource = mapper.MapTo(await resourceService.UpdateAsync(mapper.MapTo(input), cancellationToken))
        };

    [UseResolverScope]
    public async Task<ResourcePayload?> DeleteResourceAsync(
        DeleteResourceInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        new() { ClientMutationId = input.ClientMutationId, Resource = mapper.MapTo(await resourceService.DeleteAsync(input.Id, cancellationToken)) };

    [UseResolverScope]
    public async Task<ResourcesPayload?> DeleteResourcesAsync(
        DeleteResourcesInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken)
    {
        var resources = await resourceService.DeleteAsync(input.Ids.RemoveInvalidIds()!.ToList(), cancellationToken);
        return new ResourcesPayload { ClientMutationId = input.ClientMutationId, Resources = resources.Select(mapper.MapTo) };
    }

    [UseResolverScope]
    public async Task<ResourcesPayload?> ActivateResourcesAsync(
        ActivateResourcesInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken)
    {
        var resources = await resourceService.ActivateAsync(input.Ids.RemoveInvalidIds()!.ToList(), cancellationToken);
        return new ResourcesPayload { ClientMutationId = input.ClientMutationId, Resources = resources.Select(mapper.MapTo) };
    }

    [UseResolverScope]
    public async Task<ResourcesPayload?> DeactivateResourcesAsync(
        DeactivateResourcesInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken)
    {
        var resources = await resourceService.DeactivateAsync(input.Ids.RemoveInvalidIds()!.ToList(), cancellationToken);
        return new ResourcesPayload { ClientMutationId = input.ClientMutationId, Resources = resources.Select(mapper.MapTo) };
    }

    [UseResolverScope]
    public async Task<LocationPayload?> UpdateLocationOpeningHoursAsync(
        UpdateLocationOpeningHoursInput input,
        [Service] ILocationOpeningHoursService locationOpeningHoursService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = mapper.MapTo(
                await locationOpeningHoursService.UpdateOpeningHoursAsync(input.Id, mapper.MapTo(input.WeekOpeningHours)!, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<ResourcePayload?> UpdateLocationResourceAvailableHoursAsync(
        UpdateLocationResourceAvailableHoursInput input,
        [Service] IResourceAvailableHoursService resourceAvailableHoursService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Resource = mapper.MapTo(
                await resourceAvailableHoursService.UpdateAvailableHoursAsync(
                    input.Id,
                    input.OverrideAvailableHours,
                    mapper.MapTo(input.AvailableHours),
                    cancellationToken))
        };

    [UseResolverScope]
    public async Task<FloorPlanPayload> AddFloorPlanAsync(
        AddFloorPlanInput input,
        [Service] IFloorPlanService floorPlanService,
        [Service] IFloorPlanStorageService floorPlanStorageService,
        [Service] IRepositoryFactory repositoryFactory,
        [Service] IDbTransactionBuilder transactionBuilder,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IOrganizationAuthorizationService organizationAuthorizationService,
        [Service] IRandomHelper randomHelper,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(input.LocationId, cancellationToken)
                       ?? throw new LocationNotFound();

        if (!organizationAuthorizationService.CanModify(location.Organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        var existingFloorPlan = await repositoryFactory.FloorPlanRepository
            .Query(new Specification<FloorPlan>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    query.LocationId == input.LocationId &&
                                    query.FloorLevel == input.FloorLevel
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (existingFloorPlan != null)
        {
            throw new FloorPlanWithSameFloorLevelExists(input.FloorLevel);
        }

        var imageData = Convert.FromBase64String(input.ImageBase64);
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(input.ImageFileName)}";
        var contentType = GetContentType(input.ImageFileName);
        var (imagePath, thumbnailPath, width, height) = await floorPlanStorageService.SaveFloorPlanAsync(
            imageData,
            fileName,
            contentType);

        var floorPlan = new FloorPlan
        {
            Id = randomHelper.Generate(),
            LocationId = input.LocationId,
            Name = input.Name,
            FloorLevel = input.FloorLevel,
            FloorName = input.FloorName,
            ImagePath = imagePath,
            ThumbnailPath = thumbnailPath,
            Width = width,
            Height = height,
            IsActive = true
        };

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.FloorPlanRepository.Add(floorPlan);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var floorPlanModel = new Shared.Models.FloorPlan
        {
            Id = floorPlan.Id,
            CreatedAt = floorPlan.CreatedAt,
            DeletedAt = floorPlan.DeletedAt,
            ModifiedAt = floorPlan.ModifiedAt,
            Name = floorPlan.Name,
            FloorLevel = floorPlan.FloorLevel,
            FloorName = floorPlan.FloorName,
            ImagePath = floorPlan.ImagePath,
            ThumbnailPath = floorPlan.ThumbnailPath,
            Width = floorPlan.Width,
            Height = floorPlan.Height,
            IsActive = floorPlan.IsActive,
            Location = mapper.MapTo(location),
            ResourcePositions = []
        };

        return new FloorPlanPayload { ClientMutationId = input.ClientMutationId, FloorPlan = mapper.MapTo(floorPlanModel)! };
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "image/jpeg"
        };
    }

    [UseResolverScope]
    public async Task<FloorPlanPayload> UpdateFloorPlanAsync(
        UpdateFloorPlanInput input,
        [Service] IFloorPlanService floorPlanService,
        CancellationToken cancellationToken)
    {
        var floorPlan = await floorPlanService.UpdateAsync(
            input.Id,
            input.Name,
            input.FloorName,
            input.IsActive,
            null, // No image update needed here
            cancellationToken);

        return new FloorPlanPayload { ClientMutationId = input.ClientMutationId, FloorPlan = mapper.MapTo(floorPlan)! };
    }

    [UseResolverScope]
    public async Task<DeleteFloorPlanPayload> DeleteFloorPlanAsync(
        DeleteFloorPlanInput input,
        [Service] IFloorPlanService floorPlanService,
        CancellationToken cancellationToken)
    {
        await floorPlanService.DeleteAsync(input.Id, cancellationToken);

        return new DeleteFloorPlanPayload { ClientMutationId = input.ClientMutationId, Success = true };
    }

    [UseResolverScope]
    public async Task<UpdateResourcePositionsPayload> UpdateResourcePositionsAsync(
        UpdateResourcePositionsInput input,
        [Service] IFloorPlanService floorPlanService,
        CancellationToken cancellationToken)
    {
        var resourcePositions = new List<ResourcePosition>();

        foreach (var position in input.Positions)
        {
            Dictionary<string, object>? metadata = null;
            if (!string.IsNullOrWhiteSpace(position.Metadata))
            {
                try
                {
                    metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(position.Metadata);
                }
                catch
                {
                    // I don't mind ignoring metadata in case of failure
                }
            }

            var resourcePosition = await floorPlanService.UpdateResourcePositionAsync(
                position.ResourceId,
                input.FloorPlanId,
                position.X,
                position.Y,
                position.Width,
                position.Height,
                position.Shape,
                metadata,
                cancellationToken);

            resourcePositions.Add(mapper.MapTo(resourcePosition));
        }

        return new UpdateResourcePositionsPayload { ClientMutationId = input.ClientMutationId, ResourcePositions = resourcePositions };
    }

    [UseResolverScope]
    public async Task<ResourcePositionPayload> UpdateResourcePositionAsync(
        UpdateResourcePositionInput input,
        [Service] IFloorPlanService floorPlanService,
        CancellationToken cancellationToken)
    {
        Dictionary<string, object>? metadata = null;
        if (!string.IsNullOrWhiteSpace(input.Metadata))
        {
            try
            {
                metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(input.Metadata);
            }
            catch
            {
                // ignore metadata
            }
        }

        var resourcePosition = await floorPlanService.UpdateResourcePositionAsync(
            input.ResourceId,
            input.FloorPlanId,
            input.X,
            input.Y,
            input.Width,
            input.Height,
            input.Shape,
            metadata,
            cancellationToken);

        return new ResourcePositionPayload { ClientMutationId = input.ClientMutationId, ResourcePosition = mapper.MapTo(resourcePosition) };
    }

    [UseResolverScope]
    public async Task<RemoveResourcePositionPayload> RemoveResourcePositionAsync(
        RemoveResourcePositionInput input,
        [Service] IFloorPlanService floorPlanService,
        CancellationToken cancellationToken)
    {
        await floorPlanService.RemoveResourcePositionAsync(input.ResourceId, cancellationToken);

        return new RemoveResourcePositionPayload { ClientMutationId = input.ClientMutationId, Success = true };
    }
}
