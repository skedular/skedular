using Enterprise.Shared.Outbox;
using Enterprise.Shared.Temporal;
using Temporalio.Client;

namespace Organization.Shared.Workflows.Services;

public class TemporalSignalOutboxExecutorService(ITemporalClient temporalClient, ITemporalHelperService temporalHelperService)
    : ITemporalSignalOutboxExecutor
{
    private static readonly string s_scheduleRenewOrganizationOfferingCancelOfferingAsync =
        typeof(ScheduleRenewOrganizationOffering).GetMethod(nameof(ScheduleRenewOrganizationOffering.CancelOfferingAsync))!
            .ToWorkflowSignalType();

    public async Task SignalAsync(
        string workflowId,
        string signalType,
        string? executionArgs,
        WorkflowSignalOptions workflowSignalOptions,
        CancellationToken cancellationToken)
    {
        await temporalClient.Connection.ConnectAsync();
        if (signalType == s_scheduleRenewOrganizationOfferingCancelOfferingAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<ScheduleRenewOrganizationOffering>(workflowId, cancellationToken))
            {
                return;
            }

            await temporalClient
                .GetWorkflowHandle<ScheduleRenewOrganizationOffering>(workflowId)
                .SignalAsync(workflow => workflow.CancelOfferingAsync(), workflowSignalOptions);
        }
    }
}
