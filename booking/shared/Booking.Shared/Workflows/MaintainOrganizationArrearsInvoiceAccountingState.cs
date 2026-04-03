using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record MaintainOrganizationArrearsInvoiceAccountingStateInput(
    string OrganizationId,
    string OrganizationArrearsInvoiceId,
    DateTimeOffset? NotBefore = null);

[Workflow]
public class MaintainOrganizationArrearsInvoiceAccountingState
{
    private bool _refreshRequested;

    [WorkflowRun]
    public async Task ExecuteAsync(MaintainOrganizationArrearsInvoiceAccountingStateInput input)
    {
        _refreshRequested = false;

        if (input.NotBefore.HasValue)
        {
            var delay = input.NotBefore.Value - Workflow.UtcNow;
            if (delay > TimeSpan.Zero)
            {
                _ = await Workflow.WaitConditionAsync(() => _refreshRequested, delay);
            }
        }

        var result = await Workflow.ExecuteActivityAsync(
            (OrganizationArrearsBillingIntegrations activity) =>
                activity.SyncOrganizationArrearsInvoiceAccountingStateAsync(
                    new SyncOrganizationArrearsInvoiceAccountingStateInput(input.OrganizationId, input.OrganizationArrearsInvoiceId)),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(2),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
            });

        if (result.IsTerminal || result.NextSyncAt is null)
        {
            return;
        }

        throw Workflow.CreateContinueAsNewException((MaintainOrganizationArrearsInvoiceAccountingState workflow) =>
            workflow.ExecuteAsync(
                new MaintainOrganizationArrearsInvoiceAccountingStateInput(
                    input.OrganizationId,
                    input.OrganizationArrearsInvoiceId,
                    result.NextSyncAt)));
    }

    [WorkflowSignal]
    public Task RefreshNowAsync(MaintainOrganizationArrearsInvoiceAccountingStateInput _)
    {
        _refreshRequested = true;
        return Task.CompletedTask;
    }
}
