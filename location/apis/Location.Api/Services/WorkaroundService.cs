using Location.Api.Mappers;
using Location.Shared.Publishers;
using Location.Shared.Repositories;

namespace Location.Api.Services;

public interface IWorkaroundService
{
    Task RepublishLocationAsync(string locationId, CancellationToken cancellationToken);
    Task RepublishAllLocationsAsync(CancellationToken cancellationToken);
}

public class WorkaroundService(IRepositoryFactory repositoryFactory, IMapper mapper, ILocationPublisher locationPublisher) : IWorkaroundService
{
    public async Task RepublishLocationAsync(string locationId, CancellationToken cancellationToken)
    {
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null)
        {
            return;
        }

        await locationPublisher.PublishLocationsAsync([mapper.MapTo(location)], cancellationToken);
    }

    public async Task RepublishAllLocationsAsync(CancellationToken cancellationToken)
    {
        var locations = await repositoryFactory.LocationRepository.GetAllAsync(false, cancellationToken);
        await locationPublisher.PublishLocationsAsync(locations.Select(mapper.MapTo), cancellationToken);
    }
}
