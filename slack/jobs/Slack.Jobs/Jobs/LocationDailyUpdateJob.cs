using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;
using Slack.Shared.Database.Entities;
using Slack.Shared.Publishers;
using Slack.Shared.Repositories;

namespace Slack.Jobs.Jobs;

public class LocationDailyUpdateJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<LocationDailyUpdateJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory =
                    scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var slackInternalPublisher =
                    scope.ServiceProvider.GetRequiredService<ISlackInternalPublisher>();

                var now = timeProvider.GetUtcNow();
                var locations = await repositoryFactory.LocationRepository.Query(
                    new Specification<Location>
                    {
                        Criteria = query =>
                            !query.DeletedAt.HasValue &&
                            (now - query.CreatedAt).TotalHours >= 24 &&
                            query.DailyUpdateChannel != null &&
                            (!query.SlackChannelDailyUpdateLastSentAt.HasValue ||
                             (now - query.SlackChannelDailyUpdateLastSentAt.Value).TotalHours >= 23)
                    }).ToListAsync(cancellationToken);
                var locationIds = locations
                    .Where(item => now.IsMatchingHour(item.Timezone, 7))
                    .Select(item => item.Id)
                    .ToList();
                if (locationIds.Count != 0)
                {
                    await slackInternalPublisher.PublishSendWorkspaceLocationDailyUpdateMessageAsync(
                        locationIds,
                        cancellationToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(LocationDailyUpdateJob));
            }
        } while (true);
    }
}
