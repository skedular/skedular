using Booking.Shared.Services;

namespace Booking.Jobs.Services;

public sealed class ResourceBookingSlotCleanupWorkflowHostedService(
    IServiceProvider serviceProvider,
    ILogger<ResourceBookingSlotCleanupWorkflowHostedService> logger) : BackgroundService
{
    private readonly string _jobName = typeof(ResourceBookingSlotCleanupWorkflowHostedService).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var temporalService = scope.ServiceProvider.GetRequiredService<ITemporalService>();
                await temporalService.StartWorkflowCleanupUnbookedResourceBookingSlotsAsync(cancellationToken);

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
