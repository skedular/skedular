using Booking.Shared.Services;

namespace Booking.Jobs.Jobs;

public class GenerateResourceBookingSlotJob(IServiceProvider serviceProvider, ILogger<GenerateResourceBookingSlotJob> logger) : BackgroundService
{
    private readonly string _jobName = typeof(GenerateResourceBookingSlotJob).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var resourceBookingSlotsHelperService = scope.ServiceProvider.GetRequiredService<IResourceBookingSlotsHelperService>();
                await resourceBookingSlotsHelperService.GenerateAllAsync(cancellationToken);

                await Task.Delay(TimeSpan.FromDays(1), cancellationToken);
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
