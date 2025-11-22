using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Team.Shared.Workflows;
using Team.Shared.Workflows.InviteToJoinTeamExistingCustomer;
using Team.Shared.Workflows.InviteToJoinTeamNewCustomer;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Team.Shared.Publishers;

public interface ITemporalOutboxPublisher
{
    void StartWorkflowInviteToJoinTeamExistingCustomer(InviteToJoinTeamExistingCustomerInput args, IUnitOfWork unitOfWork);
    void StartWorkflowInviteToJoinTeamNewCustomer(InviteToJoinTeamNewCustomerInput args, IUnitOfWork unitOfWork);
}

public class TemporalOutboxPublisher(
    TemporalConfiguration temporalConfiguration,
    ITemporalHelperService temporalHelperService,
    ITemporalOutboxWorkflowExecutor temporalOutboxWorkflowExecutor) : ITemporalOutboxPublisher
{
    public void StartWorkflowInviteToJoinTeamExistingCustomer(InviteToJoinTeamExistingCustomerInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<InviteToJoinTeamExistingCustomer, InviteToJoinTeamExistingCustomerInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.InviteToTeamExistingCustomerPrefix}-{args.TeamId}-{args.InviteeCustomerId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);

    public void StartWorkflowInviteToJoinTeamNewCustomer(InviteToJoinTeamNewCustomerInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<InviteToJoinTeamNewCustomer, InviteToJoinTeamNewCustomerInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.InviteToTeamExistingCustomerPrefix}-{args.TeamId}-{args.InviteeCustomerEmail}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);
}
