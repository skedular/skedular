using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using Location.Shared.Services;
using Location.Shared.Workflows.GenerateLocationDailyAnalytics;
using Temporalio.Activities;

namespace Location.Shared.Activities;

public class LocationDailyAnalytics(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    ITemporalService temporalService)
{
    [Activity]
    public async Task<bool> RecordLocationDesksCountAsync(string locationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null || location.IsDeleted())
        {
            return false;
        }

        var startOfToday = timeProvider.GetUtcNow().StartOfDay();
        _ = repositoryFactory.DailyDeskCountRecordingRepository.Add(new DailyDeskCountRecording
        {
            Id = randomHelper.Generate(),
            Count = location.Resources
                .Count(item => item.OrganizationTags.Any(tag => tag.Type == OrganizationTagTypeConstants.ResourceDesk) && item.IsNotDeleted()),
            Date = startOfToday,
            Location = location
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    [Activity]
    public async Task<bool> RecordLocationRoomsCountAsync(string locationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null || location.IsDeleted())
        {
            return false;
        }

        var startOfToday = timeProvider.GetUtcNow().StartOfDay();
        _ = repositoryFactory.DailyRoomCountRecordingRepository.Add(new DailyRoomCountRecording
        {
            Id = randomHelper.Generate(),
            Count = location.Resources
                .Count(item => item.OrganizationTags.Any(tag => tag.Type == OrganizationTagTypeConstants.ResourceRoom) && item.IsNotDeleted()),
            Date = startOfToday,
            Location = location
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    [Activity]
    public async Task ExecuteNextGenerateLocationDailyAnalyticsWorkflowAsync(string locationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        await temporalService.StartWorkflowGenerateLocationDailyAnalyticsAsync(
            new GenerateLocationDailyAnalyticsInput(locationId, timeProvider.GetUtcNow().AddDays(1)),
            cancellationToken);
    }
}
