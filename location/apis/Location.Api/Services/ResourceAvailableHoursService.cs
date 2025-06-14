using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Location.Api.Mappers;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using Location.Shared.Publishers;
using Location.Shared.Repositories;

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
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    ILocationOutboxPublisher locationOutboxPublisher,
    IMapper mapper) : IResourceAvailableHoursService
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

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingResource = await repositoryFactory.ResourceRepository.GetByIdAsync(id, cancellationToken);
        if (existingResource is null)
        {
            throw new ResourceNotFound();
        }

        var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(existingResource.Location.Id, cancellationToken);
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

        var resource = mapper.MapTo(repositoryFactory.ResourceRepository.Update(existingResource));

        locationOutboxPublisher.PublishLocations([mapper.MapTo(existingLocation)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return resource;
    }
}
