using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Location.Api.Models;
using Location.Api.Services.Authorization;
using Location.Shared.Mappers;
using Location.Shared.Models;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;

namespace Location.Api.Services;

public interface ILocationPhysicalAddressService
{
    Task<Shared.Models.Location> AddAsync(LocationPhysicalAddress locationPhysicalAddress, CancellationToken cancellationToken);
    Task<Shared.Models.Location> UpdateAsync(LocationPhysicalAddressPatchRequest request, CancellationToken cancellationToken);
}

public class LocationPhysicalAddressService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IRandomHelper randomHelper,
    IEntityMapper entityMapper,
    ILocationOutboxPublisher locationOutboxPublisher,
    ICachedLocationService cachedLocationService,
    ILogger<LocationPhysicalAddressService> logger) : ILocationPhysicalAddressService
{
    public async Task<Shared.Models.Location> AddAsync(LocationPhysicalAddress locationPhysicalAddress, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(locationPhysicalAddress.Location);
        ArgumentException.ThrowIfNullOrWhiteSpace(locationPhysicalAddress.Location.Id);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(locationPhysicalAddress.Location.Id, cancellationToken) ??
                               throw new LocationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        if (!string.IsNullOrWhiteSpace(locationPhysicalAddress.Id))
        {
            var existingLocationPhysicalAddress = await repositoryFactory.LocationPhysicalAddressRepository.GetByIdAsync(
                locationPhysicalAddress.Id,
                cancellationToken);
            if (existingLocationPhysicalAddress is not null)
            {
                if (existingLocationPhysicalAddress.Location.Id != existingLocation.Id)
                {
                    throw new UnauthorizedAccessException();
                }

                return await UpdateInternalAsync(
                    locationPhysicalAddress,
                    existingLocationPhysicalAddress,
                    existingLocation,
                    cancellationToken);
            }
        }
        else
        {
            locationPhysicalAddress.Id = randomHelper.Generate();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var locationPhysicalAddressEntity = entityMapper.MapTo(locationPhysicalAddress, existingLocation);
        repositoryFactory.LocationPhysicalAddressRepository.Add(locationPhysicalAddressEntity);

        existingLocation.PhysicalAddress = locationPhysicalAddressEntity;
        var mappedLocation = entityMapper.MapTo(existingLocation);

        locationOutboxPublisher.PublishLocations([entityMapper.MapTo(existingLocation)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedLocationService.UpdateByIdAsync(existingLocation.Id, cancellationToken);

        return mappedLocation;
    }

    public async Task<Shared.Models.Location> UpdateAsync(LocationPhysicalAddressPatchRequest request, CancellationToken cancellationToken)
    {
        var editUnits = string.Join(",", request.FieldsToUpdate);
        logger.LogInformation(
            "Location physical address patch autosave started. PhysicalAddressId: {PhysicalAddressId}, EditUnits: {EditUnits}",
            request.PhysicalAddress.Id,
            editUnits);

        try
        {
            if (request.FieldsToUpdate.Contains(LocationPhysicalAddressPatchField.Address))
            {
                var updatedLocation = await UpdateAsync(request.PhysicalAddress, cancellationToken);
                logger.LogInformation(
                    "Location physical address patch autosave completed. LocationId: {LocationId}, PhysicalAddressId: {PhysicalAddressId}, EditUnits: {EditUnits}",
                    updatedLocation.Id,
                    request.PhysicalAddress.Id,
                    editUnits);
                return updatedLocation;
            }

            ArgumentNullException.ThrowIfNull(request.PhysicalAddress.Id);

            var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
            var existingLocationPhysicalAddress = await repositoryFactory.LocationPhysicalAddressRepository.GetByIdAsync(
                request.PhysicalAddress.Id,
                cancellationToken) ?? throw new LocationPhysicalAddressNotFound();
            var existingLocation =
                await repositoryFactory.LocationRepository.GetByIdAsync(existingLocationPhysicalAddress.Location.Id, cancellationToken) ??
                throw new LocationNotFound();
            if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }

            var unchangedLocation = entityMapper.MapTo(existingLocation);
            logger.LogInformation(
                "Location physical address patch autosave completed with no changes. LocationId: {LocationId}, PhysicalAddressId: {PhysicalAddressId}, EditUnits: {EditUnits}",
                unchangedLocation.Id,
                request.PhysicalAddress.Id,
                editUnits);
            return unchangedLocation;
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogWarning(
                exception,
                "Location physical address patch autosave rejected by authorization. PhysicalAddressId: {PhysicalAddressId}, EditUnits: {EditUnits}",
                request.PhysicalAddress.Id,
                editUnits);
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Location physical address patch autosave failed. PhysicalAddressId: {PhysicalAddressId}, EditUnits: {EditUnits}",
                request.PhysicalAddress.Id,
                editUnits);
            throw;
        }
    }

    private async Task<Shared.Models.Location> UpdateAsync(LocationPhysicalAddress locationPhysicalAddress, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(locationPhysicalAddress.Id);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingLocationPhysicalAddress = await repositoryFactory.LocationPhysicalAddressRepository.GetByIdAsync(
            locationPhysicalAddress.Id,
            cancellationToken) ?? throw new LocationPhysicalAddressNotFound();

        var existingLocation =
            await repositoryFactory.LocationRepository.GetByIdAsync(existingLocationPhysicalAddress.Location.Id, cancellationToken) ??
            throw new LocationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return await UpdateInternalAsync(locationPhysicalAddress, existingLocationPhysicalAddress, existingLocation, cancellationToken);
    }

    private async Task<Shared.Models.Location> UpdateInternalAsync(
        LocationPhysicalAddress locationPhysicalAddress,
        Shared.Database.Entities.LocationPhysicalAddress existingLocationPhysicalAddress,
        Shared.Database.Entities.Location existingLocation,
        CancellationToken cancellationToken)
    {
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingLocationPhysicalAddress = entityMapper.MergeTo(locationPhysicalAddress, existingLocationPhysicalAddress, existingLocation);
        repositoryFactory.LocationPhysicalAddressRepository.Update(existingLocationPhysicalAddress);

        existingLocation.PhysicalAddress = existingLocationPhysicalAddress;

        var mappedLocation = entityMapper.MapTo(existingLocation);
        locationOutboxPublisher.PublishLocations([mappedLocation], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedLocationService.UpdateByIdAsync(existingLocation.Id, cancellationToken);

        return mappedLocation;
    }
}
