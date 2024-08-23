using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;
using MsTeams.Shared.Database.Entities;
using MsTeams.Shared.Publishers;
using MsTeams.Shared.Repositories;

namespace MsTeams.Processors.Jobs;

public class RefreshAzureTenantMembersJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<RefreshAzureTenantMembersJob> logger,
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
                var msTeamsInternalPublisher =
                    scope.ServiceProvider.GetRequiredService<IMsTeamsInternalPublisher>();

                var now = timeProvider.GetUtcNow();
                var tenantIds = await repositoryFactory.AzureTenantRepository.Query(
                    new Specification<AzureTenant>
                    {
                        Criteria = query =>
                            !query.MembersLastRefreshedAt.HasValue ||
                            (now - query.MembersLastRefreshedAt.Value).TotalHours >= 24
                    }).Select(item => item.Id).ToListAsync(cancellationToken);
                if (tenantIds.Count != 0)
                {
                    await msTeamsInternalPublisher.PublishRefreshAzureTenantMembersAsync(tenantIds, cancellationToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(RefreshAzureTenantMembersJob));
            }
        } while (true);
    }
}
