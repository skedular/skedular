using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;
using MsTeams.Shared.Database.Entities;
using MsTeams.Shared.Publishers;
using MsTeams.Shared.Repositories;

namespace MsTeams.Processors.Jobs;

public class RefreshTenantMembersJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<RefreshTenantMembersJob> logger,
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
                var tenantIds = await repositoryFactory.TenantRepository.Query(
                    new Specification<Tenant>
                    {
                        Criteria = query =>
                            !query.MembersLastRefreshedAt.HasValue ||
                            (now - query.MembersLastRefreshedAt.Value).TotalHours >= 24
                    }).Select(item => item.Id).ToListAsync(cancellationToken);
                if (tenantIds.Count != 0)
                {
                    await msTeamsInternalPublisher.PublishRefreshTenantMembersAsync(tenantIds, cancellationToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(RefreshTenantMembersJob));
            }
        } while (true);
    }
}
