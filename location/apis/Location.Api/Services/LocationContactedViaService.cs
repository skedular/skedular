using Api.Shared.Services;
using Enterprise.Shared.Database;
using Location.Api.Mappers;
using Location.Api.Services.Authorization;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;

namespace Location.Api.Services;

public interface ILocationContactedViaService
{
    Task<Shared.Models.Location> ToggleContactedViaEmailAsync(string locationId, CancellationToken cancellationToken);
    Task<Shared.Models.Location> ToggleContactedViaCallAsync(string locationId, CancellationToken cancellationToken);
    Task<Shared.Models.Location> ToggleContactedViaSmsAsync(string locationId, CancellationToken cancellationToken);
    Task<Shared.Models.Location> ToggleContactedViaWhatsappAsync(string locationId, CancellationToken cancellationToken);
}

public class LocationContactedViaService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ILocationOutboxPublisher locationOutboxPublisher,
    IMapper mapper,
    ICachedLocationService cachedLocationService) : ILocationContactedViaService
{
    public async Task<Shared.Models.Location> ToggleContactedViaEmailAsync(string locationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken) ?? throw new LocationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingLocation.ContactedViaEmail = !existingLocation.ContactedViaEmail;
        var location = mapper.MapTo(repositoryFactory.LocationRepository.Update(existingLocation));

        locationOutboxPublisher.PublishLocations([location], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedLocationService.UpdateByIdAsync(existingLocation.Id, cancellationToken);

        return location;
    }

    public async Task<Shared.Models.Location> ToggleContactedViaCallAsync(string locationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken) ?? throw new LocationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingLocation.ContactedViaCall = !existingLocation.ContactedViaCall;
        var location = mapper.MapTo(repositoryFactory.LocationRepository.Update(existingLocation));

        locationOutboxPublisher.PublishLocations([location], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedLocationService.UpdateByIdAsync(existingLocation.Id, cancellationToken);

        return location;
    }

    public async Task<Shared.Models.Location> ToggleContactedViaSmsAsync(string locationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken) ?? throw new LocationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingLocation.ContactedViaSms = !existingLocation.ContactedViaSms;
        var location = mapper.MapTo(repositoryFactory.LocationRepository.Update(existingLocation));

        locationOutboxPublisher.PublishLocations([location], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedLocationService.UpdateByIdAsync(existingLocation.Id, cancellationToken);

        return location;
    }

    public async Task<Shared.Models.Location> ToggleContactedViaWhatsappAsync(string locationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken) ?? throw new LocationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingLocation.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingLocation.ContactedViaWhatsapp = !existingLocation.ContactedViaWhatsapp;
        var location = mapper.MapTo(repositoryFactory.LocationRepository.Update(existingLocation));

        locationOutboxPublisher.PublishLocations([location], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedLocationService.UpdateByIdAsync(existingLocation.Id, cancellationToken);

        return location;
    }
}
