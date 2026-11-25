using Booking.Shared.Services;
using Enterprise.Shared.Metrics;

namespace Booking.Jobs.Services;

public class SpacesBookingUsageRolloverWorkflowHostedService(
    IServiceScopeFactory serviceScopeFactory,
    IOperationTelemetry operationTelemetry,
    ILogger<SpacesBookingUsageRolloverWorkflowHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await operationTelemetry.ExecuteAsync(
                "booking.spaces_usage_rollover.ensure_workflow",
                "hosted.job",
                async ct =>
                {
                    await using var scope = serviceScopeFactory.CreateAsyncScope();
                    var temporalService = scope.ServiceProvider.GetRequiredService<ITemporalService>();
                    await temporalService.StartWorkflowRolloverSpacesBookingUsageAsync(ct);
                },
                cancellationToken);

            logger.LogInformation(
                "{EventId}: Spaces booking usage rollover workflow ensured",
                SpacesPricingLogEvents.OfferingRolloverStarted);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "{EventId}: failed to ensure Spaces booking usage rollover workflow",
                SpacesPricingLogEvents.OfferingRolloverStarted);
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
