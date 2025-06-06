using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Slack.Shared.Database.Entities;
using Slack.Shared.Repositories;
using Slack.Shared.Services;

namespace Slack.Jobs.Jobs;

public class RefreshWorkspaceMembersJob(IServiceProvider serviceProvider, TimeProvider timeProvider, ILogger<RefreshWorkspaceMembersJob> logger)
    : BackgroundService
{
    private readonly string _jobName = typeof(RefreshWorkspaceMembersJob).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var workspaceMemberService = scope.ServiceProvider.GetRequiredService<IWorkspaceMemberService>();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var now = timeProvider.GetUtcNow();
                var workspaceIds = await repositoryFactory.WorkspaceRepository.Query(
                        new Specification<Workspace>
                        {
                            Criteria = query => !query.DeletedAt.HasValue && (
                                !query.MembersLastRefreshedAt.HasValue || (now - query.MembersLastRefreshedAt.Value).TotalHours >= 24)
                        })
                    .Select(item => item.Id)
                    .ToListAsync(cancellationToken);

                foreach (var workspaceId in workspaceIds)
                {
                    logger.LogInformation("Refresh Slack Workspace Members: {workspaceId}", workspaceId);

                    await workspaceMemberService.RefreshWorkspaceMembersAsync(workspaceId, cancellationToken);
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
