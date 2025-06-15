using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Location.Api.Exceptions;
using Location.Api.Mappers;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using Location.Shared.Repositories;

namespace Location.Api.Services;

public interface IFloorPlanService
{
    Task<FloorPlan> AddAsync(
        string locationId,
        string name,
        int floorLevel,
        string? floorName,
        IFormFile imageFile,
        CancellationToken cancellationToken);

    Task<FloorPlan> UpdateAsync(
        string floorPlanId,
        string? name,
        string? floorName,
        bool? isActive,
        IFormFile? imageFile,
        CancellationToken cancellationToken);

    Task DeleteAsync(string floorPlanId, CancellationToken cancellationToken);
    Task<FloorPlan?> GetByIdAsync(string floorPlanId, CancellationToken cancellationToken);
    Task<ICollection<FloorPlan>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken);

    Task<ResourcePosition> UpdateResourcePositionAsync(
        string resourceId,
        string floorPlanId,
        int x,
        int y,
        int width,
        int height,
        string? shape,
        Dictionary<string, object>? metadata,
        CancellationToken cancellationToken);

    Task RemoveResourcePositionAsync(string resourceId, CancellationToken cancellationToken);
}

public class FloorPlanService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IFloorPlanStorageService floorPlanStorageService,
    ICachedCustomerService cachedCustomerService,
    IRandomHelper randomHelper,
    IMapper mapper) : IFloorPlanService
{
    private const long MaxFileSize = 2 * 1024 * 1024; // 2MB

    public async Task<FloorPlan> AddAsync(
        string locationId,
        string name,
        int floorLevel,
        string? floorName,
        IFormFile imageFile,
        CancellationToken cancellationToken)
    {
        if (imageFile.Length > MaxFileSize)
        {
            throw new FileSizeExceedsLimit();
        }

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken) ?? throw new LocationNotFound();

        if (!organizationAuthorizationService.CanModify(location.Organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        // Check if floor plan already exists for this floor level
        var existingFloorPlan = await repositoryFactory.FloorPlanRepository
            .GetByLocationIdAndFloorLevelAsync(locationId, floorLevel, cancellationToken);
        if (existingFloorPlan != null)
        {
            throw new FloorPlanAlreadyExistsForLevel(floorLevel);
        }

        using var stream = new MemoryStream();
        await imageFile.CopyToAsync(stream, cancellationToken);
        var imageContent = stream.ToArray();

        var fileName = $"{randomHelper.Generate()}{Path.GetExtension(imageFile.FileName)}";
        var (imageUrl, thumbnailUrl, width, height) = await floorPlanStorageService.SaveFloorPlanAsync(
            imageContent,
            fileName,
            imageFile.ContentType ?? "image/jpeg");

        var floorPlan = new Shared.Database.Entities.FloorPlan
        {
            LocationId = locationId,
            Name = name,
            FloorLevel = floorLevel,
            FloorName = floorName,
            ImagePath = imageUrl,
            ThumbnailPath = thumbnailUrl,
            Width = width,
            Height = height,
            IsActive = true
        };

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.FloorPlanRepository.Add(floorPlan);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapFloorPlan(floorPlan);
    }

    public async Task<FloorPlan> UpdateAsync(
        string floorPlanId,
        string? name,
        string? floorName,
        bool? isActive,
        IFormFile? imageFile,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var floorPlan = await repositoryFactory.FloorPlanRepository.GetByIdAsync(floorPlanId, cancellationToken) ?? throw new FloorPlanNotFound();
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(floorPlan.LocationId, cancellationToken) ??
                       throw new LocationNotFound();

        if (!organizationAuthorizationService.CanModify(location.Organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            floorPlan.Name = name;
        }

        if (floorName != null)
        {
            floorPlan.FloorName = floorName;
        }

        if (isActive.HasValue)
        {
            floorPlan.IsActive = isActive.Value;
        }

        if (imageFile != null)
        {
            if (imageFile.Length > MaxFileSize)
            {
                throw new FileSizeExceedsLimit();
            }

            await floorPlanStorageService.DeleteFloorPlanAsync(floorPlan.ImagePath, floorPlan.ThumbnailPath);

            using var stream = new MemoryStream();
            await imageFile.CopyToAsync(stream, cancellationToken);
            var imageContent = stream.ToArray();

            var fileName = $"{randomHelper.Generate()}{Path.GetExtension(imageFile.FileName)}";
            var (imageUrl, thumbnailUrl, width, height) = await floorPlanStorageService.SaveFloorPlanAsync(
                imageContent,
                fileName,
                imageFile.ContentType ?? "image/jpeg");

            floorPlan.ImagePath = imageUrl;
            floorPlan.ThumbnailPath = thumbnailUrl;
            floorPlan.Width = width;
            floorPlan.Height = height;
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.FloorPlanRepository.Update(floorPlan);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapFloorPlan(floorPlan);
    }

    public async Task DeleteAsync(string floorPlanId, CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var floorPlan = await repositoryFactory.FloorPlanRepository.GetByIdAsync(floorPlanId, cancellationToken) ?? throw new FloorPlanNotFound();
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(floorPlan.LocationId, cancellationToken) ??
                       throw new LocationNotFound();
        if (!organizationAuthorizationService.CanModify(location.Organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        await floorPlanStorageService.DeleteFloorPlanAsync(floorPlan.ImagePath, floorPlan.ThumbnailPath);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.FloorPlanRepository.Remove(floorPlan);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<FloorPlan?> GetByIdAsync(string floorPlanId, CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);

        var floorPlan = await repositoryFactory.FloorPlanRepository.GetByIdAsync(floorPlanId, cancellationToken);
        if (floorPlan == null)
        {
            return null;
        }

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(floorPlan.LocationId, cancellationToken);
        if (location == null)
        {
            return null;
        }

        if (!organizationAuthorizationService.CanView(location.Organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        return mapper.MapFloorPlan(floorPlan);
    }

    public async Task<ICollection<FloorPlan>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken) ?? throw new LocationNotFound();
        if (!organizationAuthorizationService.CanView(location.Organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        var floorPlans = await repositoryFactory.FloorPlanRepository.GetByLocationIdAsync(locationId, cancellationToken);
        return floorPlans.Select(mapper.MapFloorPlan).ToList();
    }

    public async Task<ResourcePosition> UpdateResourcePositionAsync(
        string resourceId,
        string floorPlanId,
        int x,
        int y,
        int width,
        int height,
        string? shape,
        Dictionary<string, object>? metadata,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);

        var resource = await repositoryFactory.ResourceRepository.GetByIdAsync(resourceId, cancellationToken) ?? throw new ResourceNotFound();
        var floorPlan = await repositoryFactory.FloorPlanRepository.GetByIdAsync(floorPlanId, cancellationToken) ?? throw new FloorPlanNotFound();

        if (resource.Location.Id != floorPlan.LocationId)
        {
            throw new ResourceAndFloorPlanLocationMismatch();
        }

        var location = await repositoryFactory.LocationRepository.GetByIdAsync(resource.Location.Id, cancellationToken) ??
                       throw new LocationNotFound();
        if (!organizationAuthorizationService.CanModify(location.Organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var existingPosition = await repositoryFactory.ResourcePositionRepository.GetByResourceIdAsync(resourceId, cancellationToken);

        if (existingPosition != null)
        {
            existingPosition.FloorPlanId = floorPlanId;
            existingPosition.X = x;
            existingPosition.Y = y;
            existingPosition.Width = width;
            existingPosition.Height = height;
            existingPosition.Shape = shape;
            existingPosition.Metadata = metadata;

            repositoryFactory.ResourcePositionRepository.Update(existingPosition);
        }
        else
        {
            // new position
            existingPosition = new Shared.Database.Entities.ResourcePosition
            {
                Id = randomHelper.Generate(),
                ResourceId = resourceId,
                FloorPlanId = floorPlanId,
                X = x,
                Y = y,
                Width = width,
                Height = height,
                Shape = shape,
                Metadata = metadata
            };

            repositoryFactory.ResourcePositionRepository.Add(existingPosition);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapResourcePosition(existingPosition);
    }

    public async Task RemoveResourcePositionAsync(string resourceId, CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);

        var resource = await repositoryFactory.ResourceRepository.GetByIdAsync(resourceId, cancellationToken) ?? throw new ResourceNotFound();
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(resource.Location.Id, cancellationToken) ??
                       throw new LocationNotFound();

        if (!organizationAuthorizationService.CanModify(location.Organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        var position = await repositoryFactory.ResourcePositionRepository.GetByResourceIdAsync(resourceId, cancellationToken);
        if (position == null)
        {
            return; // Already removed
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.ResourcePositionRepository.Remove(position);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
