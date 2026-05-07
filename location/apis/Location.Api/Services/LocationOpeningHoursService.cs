using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Location.Api.Services.Authorization;
using Location.Shared.Mappers;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Location.Shared.Services.Cache;

namespace Location.Api.Services;

public interface ILocationOpeningHoursService
{
    Task<Shared.Models.Location> UpdateOpeningHoursAsync(string id, WeekOpeningHours openingHours, CancellationToken cancellationToken);
}

public class LocationOpeningHoursService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    ILocationOutboxPublisher locationOutboxPublisher,
    IEntityMapper entityMapper,
    ICachedLocationService cachedLocationService) : ILocationOpeningHoursService
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
            : existingLocation.OpeningHours with { WeekOpeningHours = openingHours };

        var location = entityMapper.MapTo(repositoryFactory.LocationRepository.Update(existingLocation));

        locationOutboxPublisher.PublishLocations([location], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedLocationService.UpdateByIdAsync(location.Id, cancellationToken);

        return location;
    }
}
