using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record ProcessMarketplaceRefundInput(string RefundId, string? ActorCustomerId);

public record RefundProcessingExhaustedInput(string RefundId, string Error);

[Workflow]
public class ProcessMarketplaceRefund
{
    private static readonly RetryPolicy s_refundProviderRetryPolicy = new()
    {
        InitialInterval = TimeSpan.FromSeconds(30),
        BackoffCoefficient = 2,
        MaximumInterval = TimeSpan.FromMinutes(5),
        MaximumAttempts = 3,
        NonRetryableErrorTypes = [nameof(ArgumentException), nameof(InvalidOperationException)],
    };

    [WorkflowRun]
    public async Task ExecuteAsync(ProcessMarketplaceRefundInput input)
    {
        try
        {
            await Workflow.ExecuteActivityAsync(
                (MarketplaceRefundIntegrations activity) => activity.ProcessAsync(input),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(10),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = s_refundProviderRetryPolicy,
                });
        }
        catch (Exception exception)
        {
            await Workflow.ExecuteActivityAsync(
                (MarketplaceRefundIntegrations activity) => activity.MarkProcessingExhaustedAsync(
                    new RefundProcessingExhaustedInput(input.RefundId, exception.Message)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(2),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy
                    {
                        MaximumAttempts = 1,
                    },
                });
            throw;
        }
    }
}
