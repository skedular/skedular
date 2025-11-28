using Enterprise.Shared.Outbox;
using Enterprise.Shared.Temporal;
using Organization.Shared.Workflows.Invitation.InviteToJoinOrganizationExistingCustomer;
using Organization.Shared.Workflows.OrganizationOfferingRenewal;
using Temporalio.Client;

namespace Organization.Shared.Services;

public class TemporalSignalOutboxExecutorService(ITemporalClient temporalClient, ITemporalHelperService temporalHelperService)
    : ITemporalSignalOutboxExecutor
{
    private static readonly string s_scheduleRenewOrganizationOfferingCancelOfferingAsync =
        typeof(ScheduleRenewOrganizationOffering).GetMethod(nameof(ScheduleRenewOrganizationOffering.CancelOfferingAsync))!
            .ToWorkflowSignalType();

    private static readonly string s_inviteToJoinOrganizationExistingCustomerInvitationStatusChangedAsync =
        typeof(InviteToJoinOrganizationExistingCustomer).GetMethod(nameof(InviteToJoinOrganizationExistingCustomer.InvitationStatusChangedAsync))!
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
        else if (signalType == s_inviteToJoinOrganizationExistingCustomerInvitationStatusChangedAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<InviteToJoinOrganizationExistingCustomer>(workflowId, cancellationToken))
            {
                return;
            }

            await temporalClient
                .GetWorkflowHandle<InviteToJoinOrganizationExistingCustomer>(workflowId)
                .SignalAsync(workflow => workflow.InvitationStatusChangedAsync(), workflowSignalOptions);
        }
    }
}
