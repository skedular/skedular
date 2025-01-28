using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Location.Jobs.Jobs;

public class LocationDailyRoomCountRecorderJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<LocationDailyRoomCountRecorderJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var locationInternalPublisher = scope.ServiceProvider.GetRequiredService<ILocationInternalPublisher>();
                var yesterday = timeProvider.GetUtcNow().EndOfYesterday();
                var locationIds = await repositoryFactory.LocationRepository.Query(new Specification<Shared.Database.Entities.Location>
                {
                    Criteria = query => !query.DailyRoomCountLastRecordedAt.HasValue || query.DailyRoomCountLastRecordedAt < yesterday
                }).Select(location => location.Id).ToListAsync(cancellationToken);
                if (locationIds.Count != 0)
                {
                    await locationInternalPublisher.PublishRecordLocationDailyRoomCountAsync(locationIds, cancellationToken);
                }

                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(LocationDailyRoomCountRecorderJob));
            }
        } while (true);
    }
}
