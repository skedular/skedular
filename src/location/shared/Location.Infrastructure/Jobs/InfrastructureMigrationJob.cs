using System.Diagnostics;
using Enterprise.Shared.Hosting;
using Location.Infrastructure.Services;

namespace Location.Infrastructure.Jobs;

public class InfrastructureMigrationJob(
    IServiceProvider serviceProvider,
    IHostEnvironment hostEnvironment,
    IHostApplicationLifetimeWrapper hostApplicationLifetimeWrapper) : BackgroundService
{
    private readonly ActivitySource _activitySource = new(hostEnvironment.ApplicationName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity(hostEnvironment.ApplicationName, ActivityKind.Client);

        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationService>();

            await migrationService.MigrateAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetimeWrapper.StopApplication();
    }
}
