using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;
using MsTeams.Shared.Database.Entities;
using MsTeams.Shared.Publishers;
using MsTeams.Shared.Repositories;

namespace MsTeams.Processors.Jobs;

public class RefreshRefreshAzureTenantTeamsAndChannelsJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<RefreshRefreshAzureTenantTeamsAndChannelsJob> logger,
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
                var azureTenantIds = await repositoryFactory.AzureTenantRepository.Query(
                        new Specification<AzureTenant>
                        {
                            Criteria = query =>
                                !query.TeamsAndChannelsLastRefreshedAt.HasValue ||
                                (now - query.TeamsAndChannelsLastRefreshedAt.Value).TotalHours >= 24
                        })
                    .Select(item => item.Id)
                    .ToListAsync(cancellationToken);
                if (azureTenantIds.Count != 0)
                {
                    await msTeamsInternalPublisher.PublishRefreshAzureTenantTeamsAndChannelsAsync(
                        azureTenantIds,
                        cancellationToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(RefreshRefreshAzureTenantTeamsAndChannelsJob));
            }
        } while (true);
    }
}
