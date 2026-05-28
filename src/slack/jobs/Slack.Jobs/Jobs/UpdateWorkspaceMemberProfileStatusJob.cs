using Enterprise.Shared.Time;
using Slack.Shared.Models;
using Slack.Shared.Repositories;
using Slack.Shared.Services;
using Slack.Shared.Services.Cache;

namespace Slack.Jobs.Jobs;

public class UpdateWorkspaceMemberProfileStatusJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<UpdateWorkspaceMemberProfileStatusJob> logger) : BackgroundService
{
    private readonly string _jobName = typeof(UpdateWorkspaceMemberProfileStatusJob).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
                var workspaceMemberService = scope.ServiceProvider.GetRequiredService<IWorkspaceMemberService>();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var now = timeProvider.GetUtcNow();
                var workspaceMembers = await repositoryFactory.WorkspaceMemberRepository
                    .GetForAutomaticProfileStatusUpdateAsync(now, cancellationToken);

                var workspaceMemberIds = new List<string>();
                foreach (var workspaceMember in workspaceMembers)
                {
                    var customer = await cachedCustomerService.GetByVerifiableTokenAsync(workspaceMember.Id, cancellationToken);
                    if (customer is null)
                    {
                        continue;
                    }

                    if (!now.IsMatchingHour(customer.GetTimezone(), 7))
                    {
                        continue;
                    }

                    workspaceMemberIds.Add(workspaceMember.Id);
                }

                foreach (var workspaceMemberId in workspaceMemberIds)
                {
                    logger.LogInformation("Update Slack Workspace Members Profile Status: {workspaceMemberId}", workspaceMemberId);

                    await workspaceMemberService.UpdateWorkspaceMemberProfileStatusAsync(workspaceMemberId, cancellationToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
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
