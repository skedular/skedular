using Customer.Shared.Workflows;
using Customer.Shared.Workflows.CustomerFeedback;
using Customer.Shared.Workflows.NewCustomerJoined;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal.Configurations;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Customer.Shared.Publishers;

public interface ITemporalOutboxPublisher
{
    void StartWorkflowSubmitCustomerFeedback(SubmitCustomerFeedbackInput args, IUnitOfWork unitOfWork);
    void StartWorkflowNewCustomerJoined(NewCustomerJoinedInput args, IUnitOfWork unitOfWork);
}

public class TemporalOutboxPublisher(
    TemporalConfiguration temporalConfiguration,
    ITemporalOutboxWorkflowExecutor<SubmitCustomerFeedback> temporalOutboxSubmitCustomerFeedbackWorkflowExecutor,
    ITemporalOutboxWorkflowExecutor<NewCustomerJoined> temporalOutboxNewCustomerJoinedWorkflowExecutor) : ITemporalOutboxPublisher
{
    public void StartWorkflowSubmitCustomerFeedback(SubmitCustomerFeedbackInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxSubmitCustomerFeedbackWorkflowExecutor.Execute(
            args,
            new WorkflowOptions
            {
                Id = args.CustomerFeedbackId,
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);

    public void StartWorkflowNewCustomerJoined(NewCustomerJoinedInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxNewCustomerJoinedWorkflowExecutor.Execute(
            args,
            new WorkflowOptions
            {
                Id = $"{Constants.NewCustomerJoinedPrefix}-{args.CustomerId}",
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);
}
