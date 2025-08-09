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
    void StartWorkflowSubmitCustomerFeedback(string customerFeedbackId, IUnitOfWork unitOfWork);
    void StartWorkflowNewCustomerJoined(string customerId, IUnitOfWork unitOfWork);
}

public class TemporalOutboxPublisher(
    TemporalConfiguration temporalConfiguration,
    ITemporalOutboxWorkflowExecutor<SubmitCustomerFeedback> temporalOutboxSubmitCustomerFeedbackWorkflowExecutor,
    ITemporalOutboxWorkflowExecutor<NewCustomerJoined> temporalOutboxNewCustomerJoinedWorkflowExecutor) : ITemporalOutboxPublisher
{
    public void StartWorkflowSubmitCustomerFeedback(string customerFeedbackId, IUnitOfWork unitOfWork) =>
        temporalOutboxSubmitCustomerFeedbackWorkflowExecutor.Execute(
            new SubmitCustomerFeedbackInput(customerFeedbackId),
            new WorkflowOptions
            {
                Id = customerFeedbackId,
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);

    public void StartWorkflowNewCustomerJoined(string customerId, IUnitOfWork unitOfWork) =>
        temporalOutboxNewCustomerJoinedWorkflowExecutor.Execute(
            new NewCustomerJoinedInput(customerId),
            new WorkflowOptions
            {
                Id = $"{Constants.NewCustomerJoinedPrefix}-{customerId}",
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);
}
