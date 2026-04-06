using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record MaintainAccountingInvoiceStateInput(
    string OrganizationId,
    string LocalEntityType,
    string LocalEntityId,
    DateTimeOffset? NotBefore = null);

[Workflow]
public class MaintainAccountingInvoiceState
{
    private bool _refreshRequested;

    [WorkflowRun]
    public async Task ExecuteAsync(MaintainAccountingInvoiceStateInput input)
    {
        if (input.NotBefore.HasValue)
        {
            _refreshRequested = false;
            var delay = input.NotBefore.Value - Workflow.UtcNow;
            if (delay > TimeSpan.Zero)
            {
                _ = await Workflow.WaitConditionAsync(() => _refreshRequested, delay);
            }
        }

        var result = await Workflow.ExecuteActivityAsync(
            (InvoiceIntegrations activity) => activity.SyncAccountingInvoiceStateAsync(
                new SyncAccountingInvoiceStateInput(input.OrganizationId, input.LocalEntityType, input.LocalEntityId)),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(5),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(10) }
            });

        if (result.IsTerminal || result.NextSyncAt is null)
        {
            return;
        }

        throw Workflow.CreateContinueAsNewException((MaintainAccountingInvoiceState workflow) =>
            workflow.ExecuteAsync(
                new MaintainAccountingInvoiceStateInput(
                    input.OrganizationId,
                    input.LocalEntityType,
                    input.LocalEntityId,
                    result.NextSyncAt)));
    }

    [WorkflowSignal]
    public Task RefreshNowAsync(MaintainAccountingInvoiceStateInput _)
    {
        _refreshRequested = true;
        return Task.CompletedTask;
    }
}
