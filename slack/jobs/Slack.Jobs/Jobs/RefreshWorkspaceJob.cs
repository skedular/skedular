using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Slack.Shared.Database.Entities;
using Slack.Shared.Publishers;
using Slack.Shared.Repositories;

namespace Slack.Jobs.Jobs;

public class RefreshWorkspaceJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<RefreshWorkspaceJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var slackInternalPublisher = scope.ServiceProvider.GetRequiredService<ISlackInternalPublisher>();
                var now = timeProvider.GetUtcNow();
                var workspaceIds = await repositoryFactory.WorkspaceRepository.Query(
                        new Specification<Workspace>
                        {
                            Criteria = query =>
                                !query.LastRefreshedAt.HasValue ||
                                (now - query.LastRefreshedAt.Value).TotalHours >= 24
                        })
                    .Select(item => item.Id)
                    .ToListAsync(cancellationToken);
                if (workspaceIds.Count != 0)
                {
                    await slackInternalPublisher.PublishRefreshWorkspaceAsync(workspaceIds, cancellationToken);
                }

                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(RefreshWorkspaceJob));
            }
        } while (true);
    }
}
