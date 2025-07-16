using Organization.Shared.Workflows.OrganizationOfferingRenewal;
using Temporalio.Workflows;

namespace Organization.Shared.Workflows.Invitation.NonExistingCustomer;

[Workflow]
public class InviteNonExistingCustomerToJoinOrganization
{
    [WorkflowRun]
    public async Task ExecuteAsync(ScheduleRenewOrganizationOfferingInput args)
    {
    }
}
