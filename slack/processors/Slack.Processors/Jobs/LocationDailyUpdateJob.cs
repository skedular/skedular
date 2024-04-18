using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;
using Slack.Shared.Database.Entities;
using Slack.Shared.Publishers;
using Slack.Shared.Repositories;

namespace Slack.Processors.Jobs;

public class LocationDailyUpdateJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<LocationDailyUpdateJob> logger,
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
                             (now - query.SlackChannelDailyUpdateLastSentAt.Value).TotalHours >= 24)
                    }).ToListAsync(cancellationToken);
                var locationIds = locations
                    .Where(item => now.IsMatchingHour(item.Timezone, 7))
                    .Select(item => item.Id)
                    .ToList();
                if (locationIds.Count != 0)
                {
                    await slackInternalPublisher.PublishWorkspaceLocationDailyUpdateMessageAsync(locationIds,
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
                logger.LogError(ex, "Failed to run job: {job}", nameof(RefreshWorkspaceMembersJob));
            }
        } while (true);
    }
}
