using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Location.Processors.Jobs;

public class LocationDailyDeskCountRecorderJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<LocationDailyDeskCountRecorderJob> logger,
    ITimeHelper timeHelper) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await timeHelper.RandomSleepWhileStartingUpAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory =
                    scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var locationInternalPublisher =
                    scope.ServiceProvider.GetRequiredService<ILocationInternalPublisher>();

                var yesterday = timeProvider.GetUtcNow().EndOfYesterday();
                var locationIds = await repositoryFactory.LocationRepository.Query(
                    new Specification<Shared.Database.Entities.Location>
                    {
                        Criteria = query =>
                            !query.DailyDeskCountLastRecordedAt.HasValue ||
                            query.DailyDeskCountLastRecordedAt < yesterday
                    }).Select(location => location.Id).ToListAsync(cancellationToken);
                if (locationIds.Count != 0)
                {
                    await locationInternalPublisher.PublishRecordLocationDailyDeskCountAsync(
                        locationIds,
                        cancellationToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(LocationDailyDeskCountRecorderJob));
            }
        } while (true);
    }
}
