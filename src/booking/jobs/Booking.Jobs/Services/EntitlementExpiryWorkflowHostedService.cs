using Booking.Shared.Services;

namespace Booking.Jobs.Services;

public sealed class EntitlementExpiryWorkflowHostedService(
    IServiceProvider serviceProvider,
    ILogger<EntitlementExpiryWorkflowHostedService> logger) : BackgroundService
{
    private readonly string _jobName = typeof(EntitlementExpiryWorkflowHostedService).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var temporalService = scope.ServiceProvider.GetRequiredService<ITemporalService>();
                await temporalService.StartWorkflowExpireEntitlementsAsync(cancellationToken);

                break;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", _jobName);
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        } while (true);
    }
}
