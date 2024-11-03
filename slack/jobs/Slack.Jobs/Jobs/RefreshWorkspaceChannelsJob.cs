using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;
using Slack.Shared.Database.Entities;
using Slack.Shared.Publishers;
using Slack.Shared.Repositories;

namespace Slack.Jobs.Jobs;

public class RefreshWorkspaceChannelsJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<RefreshWorkspaceChannelsJob> logger,
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
                var workspaceIds = await repositoryFactory.WorkspaceRepository.Query(
                        new Specification<Workspace>
                        {
                            Criteria = query =>
                                !query.ChannelsLastRefreshedAt.HasValue ||
                                (now - query.ChannelsLastRefreshedAt.Value).TotalHours >= 24
                        })
                    .Select(item => item.Id)
                    .ToListAsync(cancellationToken);
                if (workspaceIds.Count != 0)
                {
                    await slackInternalPublisher.PublishRefreshWorkspaceChannelsAsync(workspaceIds, cancellationToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(RefreshWorkspaceChannelsJob));
            }
        } while (true);
    }
}
