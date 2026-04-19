using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Slack.Shared.Repositories;
using Slack.Shared.Services;

namespace Slack.Jobs.Jobs;

public class LocationDailyUpdateJob(IServiceProvider serviceProvider, TimeProvider timeProvider, ILogger<LocationDailyUpdateJob> logger)
    : BackgroundService
{
    private readonly string _jobName = typeof(LocationDailyUpdateJob).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var locationDailyUpdaterService = scope.ServiceProvider.GetRequiredService<ILocationDailyUpdaterService>();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var now = timeProvider.GetUtcNow();
                var locations = await repositoryFactory.LocationRepository.GetDueForDailyUpdateAsync(now, cancellationToken);

                foreach (var locationId in locations.Where(item => now.IsMatchingHour(item.Timezone, 7)).Select(item => item.Id))
                {
                    logger.LogInformation("Sending location daily update for location: {locationId}", locationId);

                    await locationDailyUpdaterService.SendDailyUpdateAsync(locationId, cancellationToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", _jobName);
            }
        } while (true);
    }
}
