using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Location.Api.Mappers;
using Location.Api.Services.Authorization;
using Location.Shared.Publishers;
using Location.Shared.Repositories;

namespace Location.Api.Services;

public interface ILocationOpeningHoursService
{
    Task<Shared.Models.Location> UpdateOpeningHoursAsync(string id, WeekOpeningHours weekOpeningHours, CancellationToken cancellationToken);
}

public class LocationOpeningHoursService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    ILocationOutboxPublisher locationOutboxPublisher,
    IMapper mapper) : ILocationOpeningHoursService
{
    public async Task<Shared.Models.Location> UpdateOpeningHoursAsync(
        string id,
        WeekOpeningHours weekOpeningHours,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(id, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (!organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!organizationAuthorizationService.CanModify(existingLocation.Organization, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingLocation.OpeningHours = existingLocation.OpeningHours is null
            ? new OpeningHours(weekOpeningHours, [], [])
            : existingLocation.OpeningHours with { WeekOpeningHours = weekOpeningHours };

        var location = mapper.MapTo(repositoryFactory.LocationRepository.Update(existingLocation));

        await locationOutboxPublisher.PublishLocationsAsync([location], repositoryFactory.UnitOfWork, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return location;
    }
}
