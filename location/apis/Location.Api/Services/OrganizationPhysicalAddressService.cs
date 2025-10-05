using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Location.Api.Mappers;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;

namespace Location.Api.Services;

public interface ILocationPhysicalAddressService
{
    Task<Shared.Models.Location> AddAsync(LocationPhysicalAddress locationPhysicalAddress, CancellationToken cancellationToken);
    Task<Shared.Models.Location> UpdateAsync(LocationPhysicalAddress locationPhysicalAddress, CancellationToken cancellationToken);
}

public class LocationPhysicalAddressService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IRandomHelper randomHelper,
    IMapper mapper,
    ILocationOutboxPublisher locationOutboxPublisher,
    ICachedLocationService cachedLocationService) : ILocationPhysicalAddressService
{
    public async Task<Shared.Models.Location> AddAsync(LocationPhysicalAddress locationPhysicalAddress, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(locationPhysicalAddress.Location);
        ArgumentException.ThrowIfNullOrWhiteSpace(locationPhysicalAddress.Location.Id);

        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(locationPhysicalAddress.Location.Id, cancellationToken) ??
                               throw new LocationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customer.Id, cancellationToken))
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

        var locationPhysicalAddressEntity = mapper.MapTo(locationPhysicalAddress, existingLocation);
        repositoryFactory.LocationPhysicalAddressRepository.Add(locationPhysicalAddressEntity);

        existingLocation.PhysicalAddress = locationPhysicalAddressEntity;
        var mappedLocation = mapper.MapTo(existingLocation);

        locationOutboxPublisher.PublishLocations([mapper.MapTo(existingLocation)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedLocationService.UpdateByIdAsync(locationPhysicalAddress.Location.Id, cancellationToken);

        return mappedLocation;
    }

    public async Task<Shared.Models.Location> UpdateAsync(LocationPhysicalAddress locationPhysicalAddress, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(locationPhysicalAddress.Id);

        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var existingLocationPhysicalAddress = await repositoryFactory.LocationPhysicalAddressRepository.GetByIdAsync(
            locationPhysicalAddress.Id,
            cancellationToken) ?? throw new LocationPhysicalAddressNotFound();

        var existingLocation =
            await repositoryFactory.LocationRepository.GetByIdAsync(existingLocationPhysicalAddress.Location.Id, cancellationToken) ??
            throw new LocationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customer.Id, cancellationToken))
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

        existingLocationPhysicalAddress = mapper.MergeTo(locationPhysicalAddress, existingLocationPhysicalAddress, existingLocation);
        repositoryFactory.LocationPhysicalAddressRepository.Update(existingLocationPhysicalAddress);

        existingLocation.PhysicalAddress = existingLocationPhysicalAddress;

        var mappedLocation = mapper.MapTo(existingLocation);
        locationOutboxPublisher.PublishLocations([mappedLocation], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedLocationService.UpdateByIdAsync(locationPhysicalAddress.Location.Id, cancellationToken);

        return mappedLocation;
    }
}
