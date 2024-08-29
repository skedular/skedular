using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;
using Slack.Shared.Database.Entities;
using Slack.Shared.Publishers;
using Slack.Shared.Repositories;

namespace Slack.Processors.Jobs;

public class TeamDailyUpdateJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<TeamDailyUpdateJob> logger,
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
                var teams = await repositoryFactory.TeamRepository.Query(
                    new Specification<Team>
                    {
                        Criteria = query =>
                            !query.DeletedAt.HasValue &&
                            (now - query.CreatedAt).TotalHours >= 24 &&
                            query.DailyUpdateChannel != null &&
                            (!query.SlackChannelDailyUpdateLastSentAt.HasValue ||
                             (now - query.SlackChannelDailyUpdateLastSentAt.Value).TotalHours >= 24)
                    }).ToListAsync(cancellationToken);
                var teamIds = teams
                    .Where(item => now.IsMatchingHour(item.Timezone, 7))
                    .Select(item => item.Id)
                    .ToList();
                if (teamIds.Count != 0)
                {
                    await slackInternalPublisher.PublishSendWorkspaceTeamDailyUpdateMessageAsync(teamIds,
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
