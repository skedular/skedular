using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Processors.Jobs;

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
                    scope.ServiceProvider.GetRequiredService<IOrganizationInternalPublisher>();

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

                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
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
