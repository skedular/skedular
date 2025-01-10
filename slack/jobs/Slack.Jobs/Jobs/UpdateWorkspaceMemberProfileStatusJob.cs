using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;
using Slack.Shared.Models;
using Slack.Shared.Publishers;
using Slack.Shared.Repositories;
using WorkspaceMember = Slack.Shared.Database.Entities.WorkspaceMember;

namespace Slack.Jobs.Jobs;

public class UpdateWorkspaceMemberProfileStatusJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<UpdateWorkspaceMemberProfileStatusJob> logger) : BackgroundService
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
                var workspaceMembers = await repositoryFactory.WorkspaceMemberRepository.Query(
                        new Specification<WorkspaceMember>
                        {
                            Criteria = query =>
                                !query.DeletedAt.HasValue &&
                                query.AutomaticallyUpdateProfileStatus.HasValue &&
                                query.AutomaticallyUpdateProfileStatus.Value &&
                                (!query.LastProfileStatusUpdatedAt.HasValue ||
                                 (now - query.LastProfileStatusUpdatedAt.Value).TotalHours >= 24) &&
                                EF.Functions.ILike(query.Workspace.AuthedUserScope, "%users.profile:read%") &&
                                EF.Functions.ILike(query.Workspace.AuthedUserScope, "%users.profile:write%")
                        }.AddInclude(query => query.Workspace))
                    .ToListAsync(cancellationToken);
                var workspaceMemberIds = new List<string>();
                foreach (var workspaceMember in workspaceMembers)
                {
                    var customerEntity = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(
                        workspaceMember.Id,
                        cancellationToken);
                    if (customerEntity is null)
                    {
                        continue;
                    }

                    if (!now.IsMatchingHour(customerEntity.GetTimezone(), 7))
                    {
                        continue;
                    }

                    workspaceMemberIds.Add(workspaceMember.Id);
                }

                if (workspaceMemberIds.Count != 0)
                {
                    await slackInternalPublisher.PublishUpdateWorkspaceMemberProfileStatusAsync(
                        workspaceMemberIds,
                        cancellationToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(UpdateWorkspaceMemberProfileStatusJob));
            }
        } while (true);
    }
}
