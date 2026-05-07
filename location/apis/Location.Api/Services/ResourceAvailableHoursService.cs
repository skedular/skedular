using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
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
}

public class ResourceAvailableHoursService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    ILocationOutboxPublisher locationOutboxPublisher,
    IEntityMapper entityMapper) : IResourceAvailableHoursService
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
}
