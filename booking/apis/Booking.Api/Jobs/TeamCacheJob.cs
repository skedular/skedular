using Booking.Api.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Api.Jobs;

public class TeamCacheJob(IServiceProvider serviceProvider, ILogger<TeamCacheJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var cachedTeamService = scope.ServiceProvider.GetRequiredService<ICachedTeamService>();
                var teams = await repositoryFactory.TeamRepository
                    .Query(new Specification<Team> { Criteria = query => !query.DeletedAt.HasValue })
                    .ToListAsync(cancellationToken);

                foreach (var team in teams)
                {
                    logger.LogTrace("Caching team by id {id}", team.Id);
                    _ = await cachedTeamService.GetByIdAsync(team.Id, cancellationToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(TeamCacheJob));
            }
        } while (true);
    }
}
