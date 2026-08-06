using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Location.Api.Models;
using Location.Api.Services.Authorization;
using Location.Shared.Mappers;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;

namespace Location.Api.Services;

public interface ILocationOpeningHoursService
{
    Task<Shared.Models.Location> UpdateOpeningHoursAsync(string id, WeekOpeningHours openingHours, CancellationToken cancellationToken);
    Task<Shared.Models.Location> UpdateOpeningHoursAsync(LocationOpeningHoursPatchRequest request, CancellationToken cancellationToken);
}

public class LocationOpeningHoursService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    ILocationOutboxPublisher locationOutboxPublisher,
    IEntityMapper entityMapper,
    ICachedLocationService cachedLocationService,
    ILogger<LocationOpeningHoursService> logger) : ILocationOpeningHoursService
{
    public async Task<Shared.Models.Location> UpdateOpeningHoursAsync(string id, WeekOpeningHours openingHours, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(id, cancellationToken) ?? throw new LocationNotFound();
        if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingLocation.OpeningHours = existingLocation.OpeningHours is null
            ? new OpeningHours(openingHours, [], [])
            : existingLocation.OpeningHours with
            {
                WeekOpeningHours = openingHours,
            };

        var location = entityMapper.MapTo(repositoryFactory.LocationRepository.Update(existingLocation));

        locationOutboxPublisher.PublishLocations([location], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedLocationService.UpdateByIdAsync(location.Id, cancellationToken);

        return location;
    }

    public async Task<Shared.Models.Location> UpdateOpeningHoursAsync(LocationOpeningHoursPatchRequest request, CancellationToken cancellationToken)
    {
        var editUnits = string.Join(",", request.FieldsToUpdate);
        logger.LogInformation(
            "Location opening hours patch autosave started. LocationId: {LocationId}, EditUnits: {EditUnits}",
            request.Id,
            editUnits);

        try
        {
            if (request.FieldsToUpdate.Contains(LocationOpeningHoursPatchField.WeekOpeningHours))
            {
                var updatedHoursLocation = await UpdateOpeningHoursAsync(request.Id, request.WeekOpeningHours, cancellationToken);
                logger.LogInformation(
                    "Location opening hours patch autosave completed. LocationId: {LocationId}, EditUnits: {EditUnits}",
                    updatedHoursLocation.Id,
                    editUnits);
                return updatedHoursLocation;
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(request.Id);

            var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
            var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(request.Id, cancellationToken) ??
                                   throw new LocationNotFound();
            if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }

            var unchangedLocation = entityMapper.MapTo(existingLocation);
            logger.LogInformation(
                "Location opening hours patch autosave completed with no changes. LocationId: {LocationId}, EditUnits: {EditUnits}",
                unchangedLocation.Id,
                editUnits);
            return unchangedLocation;
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogWarning(
                exception,
                "Location opening hours patch autosave rejected by authorization. LocationId: {LocationId}, EditUnits: {EditUnits}",
                request.Id,
                editUnits);
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Location opening hours patch autosave failed. LocationId: {LocationId}, EditUnits: {EditUnits}",
                request.Id,
                editUnits);
            throw;
        }
    }
}
