using Booking.Shared.Activities;
using Booking.Shared.Models.Entitlements;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public sealed record PrepareEntitlementRenewalInput(
    string EntitlementId,
    DateTimeOffset PaymentExpiry);

[Workflow]
public sealed class PrepareEntitlementRenewal
{
    [WorkflowRun]
    public Task<EntitlementPurchase?> ExecuteAsync(PrepareEntitlementRenewalInput input) =>
        Workflow.ExecuteActivityAsync(
            (EntitlementRenewalIntegrations activity) =>
                activity.PreparePendingRenewalAsync(
                    new PrepareEntitlementRenewalActivityInput(input.EntitlementId, input.PaymentExpiry)),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(2),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy
                {
                    InitialInterval = TimeSpan.FromSeconds(5),
                    MaximumAttempts = 0,
                    MaximumInterval = TimeSpan.FromMinutes(1),
                },
            });
}
