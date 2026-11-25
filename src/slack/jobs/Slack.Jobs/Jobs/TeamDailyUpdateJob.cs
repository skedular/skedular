using Enterprise.Shared.Metrics;
using Enterprise.Shared.Time;
using Slack.Shared.Repositories;
using Slack.Shared.Services;

namespace Slack.Jobs.Jobs;

public class TeamDailyUpdateJob(
    IServiceProvider serviceProvider,
    TimeProvider timeProvider,
    IOperationTelemetry operationTelemetry,
    ILogger<TeamDailyUpdateJob> logger) : BackgroundService
{
    private readonly string _jobName = typeof(TeamDailyUpdateJob).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await operationTelemetry.ExecuteAsync(
                    "slack.team_daily_update.loop",
                    "hosted.job",
                    async ct =>
                    {
                        await using var scope = serviceProvider.CreateAsyncScope();
                        var teamDailyUpdaterService = scope.ServiceProvider.GetRequiredService<ITeamDailyUpdaterService>();
                        var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                        var now = timeProvider.GetUtcNow();
                        var teams = await repositoryFactory.TeamRepository.GetDueForDailyUpdateAsync(now, ct);

                        foreach (var teamId in teams.Where(item => now.IsMatchingHour(item.Timezone, 7)).Select(item => item.Id))
                        {
                            logger.LogInformation("Sending team daily update for location: {teamId}", teamId);
                            await teamDailyUpdaterService.SendDailyUpdateAsync(teamId, ct);
                        }

                        await Task.Delay(TimeSpan.FromSeconds(30), ct);
                    },
                    cancellationToken);
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
