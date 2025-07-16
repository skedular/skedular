using Organization.Shared.Workflows.OrganizationOfferingRenewal;
using Temporalio.Workflows;

namespace Organization.Shared.Workflows.Invitation.ExistingCustomer;

[Workflow]
public class InviteExistingCustomerToJoinOrganization
{
    [WorkflowRun]
    public async Task ExecuteAsync(ScheduleRenewOrganizationOfferingInput args)
    {
    }
}
