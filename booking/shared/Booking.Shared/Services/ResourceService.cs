using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;

namespace Booking.Shared.Services;

public interface IResourceService
{
    Task<ICollection<Resource>> GetResourceEntitiesAndValidateAvailabilityAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> resourceIds,
        CancellationToken cancellationToken);
}

public class ResourceService(IRepositoryFactory repositoryFactory) : IResourceService
{
    public async Task<ICollection<Resource>> GetResourceEntitiesAndValidateAvailabilityAsync(
        DateTimeOffset from,
        DateTimeOffset until,
        ICollection<string> resourceIds,
        CancellationToken cancellationToken)
    {
        if (resourceIds.Count == 0)
        {
            return [];
        }

        var availableResources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
            null,
            null,
            from,
            until,
            resourceIds,
            [],
            [],
            cancellationToken);

        return availableResources.Count != resourceIds.Count || !availableResources.All(item => resourceIds.Contains(item.Id))
            ? throw new ResourceNotAvailable()
            : availableResources;
    }
}
