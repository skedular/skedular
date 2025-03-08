using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Slack.Shared.Database.Entities;
using Slack.Shared.Repositories;
using Slack.Shared.Services;

namespace Slack.Jobs.Jobs;

public class RefreshWorkspaceChannelsJob(IServiceProvider serviceProvider, TimeProvider timeProvider, ILogger<RefreshWorkspaceChannelsJob> logger)
    : BackgroundService
{
    private readonly string _jobName = typeof(RefreshWorkspaceChannelsJob).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var workspaceChannelService = scope.ServiceProvider.GetRequiredService<IWorkspaceChannelService>();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var now = timeProvider.GetUtcNow();
                var workspaceIds = await repositoryFactory.WorkspaceRepository.Query(
                        new Specification<Workspace>
                        {
                            Criteria = query =>
                                !query.ChannelsLastRefreshedAt.HasValue || (now - query.ChannelsLastRefreshedAt.Value).TotalHours >= 24
                        })
                    .Select(item => item.Id)
                    .ToListAsync(cancellationToken);

                foreach (var workspaceId in workspaceIds)
                {
                    logger.LogInformation("Refresh Slack Workspace Channels: {workspaceId}", workspaceId);

                    await workspaceChannelService.RefreshWorkspaceChannelsAsync(workspaceId, cancellationToken);
                }

                await Task.Delay(TimeSpan.FromHours(1), cancellationToken);
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
