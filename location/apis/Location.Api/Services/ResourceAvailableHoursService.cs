using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Location.Api.Models;
using Location.Api.Services.Authorization;
using Location.Shared.Mappers;
using Location.Shared.Models;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;

namespace Location.Api.Services;

public interface IResourceAvailableHoursService
{
    Task<Resource> UpdateAvailableHoursAsync(
        string id,
        bool overrideAvailableHours,
        WeekOpeningHours? availableHours,
        CancellationToken cancellationToken);

    Task<Resource> UpdateAvailableHoursAsync(ResourceAvailableHoursPatchRequest request, CancellationToken cancellationToken);
}

public class ResourceAvailableHoursService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    ILocationOutboxPublisher locationOutboxPublisher,
    IEntityMapper entityMapper,
    ILogger<ResourceAvailableHoursService> logger) : IResourceAvailableHoursService
{
    public async Task<Resource> UpdateAvailableHoursAsync(
        string id,
        bool overrideAvailableHours,
        WeekOpeningHours? availableHours,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        if (overrideAvailableHours)
        {
            ArgumentNullException.ThrowIfNull(availableHours);
        }

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingResource = await repositoryFactory.ResourceRepository.GetByIdAsync(id, cancellationToken) ?? throw new ResourceNotFound();
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(existingResource.Location.Id, cancellationToken) ??
                               throw new LocationNotFound();
        if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (overrideAvailableHours)
        {
            existingResource.IsAvailableHoursOverridden = true;
            existingResource.AvailableHours = existingResource.AvailableHours is null
                ? new OpeningHours(availableHours!, [], [])
                : existingResource.AvailableHours with { WeekOpeningHours = availableHours! };
        }
        else
        {
            existingResource.IsAvailableHoursOverridden = false;
            existingResource.AvailableHours = null;
        }

        var resource = entityMapper.MapTo(repositoryFactory.ResourceRepository.Update(existingResource));

        locationOutboxPublisher.PublishLocations([entityMapper.MapTo(existingLocation)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return resource;
    }

    public async Task<Resource> UpdateAvailableHoursAsync(ResourceAvailableHoursPatchRequest request, CancellationToken cancellationToken)
    {
        var editUnits = string.Join(",", request.FieldsToUpdate);
        logger.LogInformation(
            "Resource available hours patch autosave started. ResourceId: {ResourceId}, EditUnits: {EditUnits}",
            request.Id,
            editUnits);

        try
        {
            if (request.FieldsToUpdate.Contains(LocationResourceAvailableHoursPatchField.AvailableHours))
            {
                var updatedResource =
                    await UpdateAvailableHoursAsync(request.Id, request.OverrideAvailableHours, request.AvailableHours, cancellationToken);
                logger.LogInformation(
                    "Resource available hours patch autosave completed. ResourceId: {ResourceId}, EditUnits: {EditUnits}",
                    updatedResource.Id,
                    editUnits);
                return updatedResource;
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(request.Id);

            var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
            var existingResource = await repositoryFactory.ResourceRepository.GetByIdAsync(request.Id, cancellationToken) ??
                                   throw new ResourceNotFound();
            var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(existingResource.Location.Id, cancellationToken) ??
                                   throw new LocationNotFound();
            if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }

            var unchangedResource = entityMapper.MapTo(existingResource);
            logger.LogInformation(
                "Resource available hours patch autosave completed with no changes. ResourceId: {ResourceId}, EditUnits: {EditUnits}",
                unchangedResource.Id,
                editUnits);
            return unchangedResource;
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogWarning(
                exception,
                "Resource available hours patch autosave rejected by authorization. ResourceId: {ResourceId}, EditUnits: {EditUnits}",
                request.Id,
                editUnits);
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Resource available hours patch autosave failed. ResourceId: {ResourceId}, EditUnits: {EditUnits}",
                request.Id,
                editUnits);
            throw;
        }
    }
}
