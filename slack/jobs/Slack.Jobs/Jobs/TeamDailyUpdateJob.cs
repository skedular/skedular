using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;
using Slack.Shared.Database.Entities;
using Slack.Shared.Repositories;
using Slack.Shared.Services;

namespace Slack.Jobs.Jobs;

public class TeamDailyUpdateJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    ILogger<TeamDailyUpdateJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var teamDailyUpdaterService = scope.ServiceProvider.GetRequiredService<ITeamDailyUpdaterService>();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var now = timeProvider.GetUtcNow();
                var teams = await repositoryFactory.TeamRepository.Query(
                    new Specification<Team>
                    {
                        Criteria = query =>
                            !query.DeletedAt.HasValue &&
                            (now - query.CreatedAt).TotalHours >= 24 &&
                            query.DailyUpdateChannel != null &&
                            (!query.SlackChannelDailyUpdateLastSentAt.HasValue ||
                             (now - query.SlackChannelDailyUpdateLastSentAt.Value).TotalHours >= 23)
                    }).ToListAsync(cancellationToken);

                foreach (var teamId in teams.Where(item => now.IsMatchingHour(item.Timezone, 7)).Select(item => item.Id))
                {
                    logger.LogInformation("Sending team daily update for location: {teamId}", teamId);

                    await teamDailyUpdaterService.SendDailyUpdateAsync(teamId, cancellationToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(TeamDailyUpdateJob));
            }
        } while (true);
    }
}
