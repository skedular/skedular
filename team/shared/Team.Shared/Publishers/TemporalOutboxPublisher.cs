using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal.Configurations;
using Team.Shared.Activities;
using Team.Shared.Workflows;
using Team.Shared.Workflows.InviteToJoinTeamExistingCustomer;
using Team.Shared.Workflows.InviteToJoinTeamNewCustomer;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Team.Shared.Publishers;

public interface ITemporalOutboxPublisher
{
    void StartWorkflowInviteToJoinTeamExistingCustomer(string teamId, string inviterCustomerId, string inviteeCustomerId, IUnitOfWork unitOfWork);
    void StartWorkflowInviteToJoinTeamNewCustomer(string teamId, string inviterCustomerId, string inviteeCustomerEmail, IUnitOfWork unitOfWork);
}

public class TemporalOutboxPublisher(
    TemporalConfiguration temporalConfiguration,
    ITemporalOutboxWorkflowExecutor<InviteToJoinTeamExistingCustomer> temporalOutboxInviteToJoinTeamExistingCustomerExecutor,
    ITemporalOutboxWorkflowExecutor<InviteToJoinTeamNewCustomer> temporalOutboxInviteToJoinTeamNewCustomerWorkflowExecutor) : ITemporalOutboxPublisher
{
    public void StartWorkflowInviteToJoinTeamExistingCustomer(
        string teamId,
        string inviterCustomerId,
        string inviteeCustomerId,
        IUnitOfWork unitOfWork) =>
        temporalOutboxInviteToJoinTeamExistingCustomerExecutor.Execute(
            new SendInviteCustomerToJoinTeamExistingCustomerInput(teamId, inviterCustomerId, inviteeCustomerId),
            new WorkflowOptions
            {
                Id = $"{Constants.InviteToTeamExistingCustomerPrefix}-{teamId}-{inviteeCustomerId}",
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);

    public void StartWorkflowInviteToJoinTeamNewCustomer(
        string teamId,
        string inviterCustomerId,
        string inviteeCustomerEmail,
        IUnitOfWork unitOfWork) =>
        temporalOutboxInviteToJoinTeamNewCustomerWorkflowExecutor.Execute(
            new InviteToJoinTeamNewCustomerInput(teamId, inviterCustomerId, inviteeCustomerEmail),
            new WorkflowOptions
            {
                Id = $"{Constants.InviteToTeamExistingCustomerPrefix}-{teamId}-{inviteeCustomerEmail}",
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);
}
