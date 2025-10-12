using Api.Shared.Services;
using Enterprise.Shared.Database;
using Location.Api.Mappers;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Location.Shared.Services;
using Location.Shared.Workflows.GenerateLocationDailyAnalytics;

namespace Location.Api.Services;

public interface IWorkaroundService
{
    Task RepublishLocationAsync(string locationId, CancellationToken cancellationToken);
    Task RepublishAllLocationsAsync(CancellationToken cancellationToken);
    Task RegenerateAllDailyAnalyticsAsync(CancellationToken cancellationToken);
    Task RegenerateDailyAnalyticsAsync(string locationId, CancellationToken cancellationToken);
}

public class WorkaroundService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    ILocationPublisher locationPublisher,
    ITemporalService temporalService) : IWorkaroundService
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
        var locations = await repositoryFactory.LocationRepository.GetAllUntrackedAsync(false, cancellationToken);
        await locationPublisher.PublishLocationsAsync(locations.Select(mapper.MapTo), cancellationToken);
    }

    public async Task RegenerateAllDailyAnalyticsAsync(CancellationToken cancellationToken)
    {
        var locations = await repositoryFactory.LocationRepository.GetAllUntrackedAsync(false, cancellationToken);

        foreach (var location in locations.Where(item =>
                     item.Organization?.UniqueAlphanumericName != Constants.SkedularPublicLocationsUniqueAlphanumericName))
        {
            await temporalService.StartWorkflowGenerateLocationDailyAnalyticsAsync(
                new GenerateLocationDailyAnalyticsInput(location.Id, null),
                cancellationToken);
        }
    }

    public async Task RegenerateDailyAnalyticsAsync(string locationId, CancellationToken cancellationToken)
    {
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null || location.IsDeleted())
        {
            return;
        }

        if (location.Organization?.UniqueAlphanumericName == Constants.SkedularPublicLocationsUniqueAlphanumericName)
        {
            return;
        }

        await temporalService.StartWorkflowGenerateLocationDailyAnalyticsAsync(
            new GenerateLocationDailyAnalyticsInput(location.Id, null),
            cancellationToken);
    }
}
