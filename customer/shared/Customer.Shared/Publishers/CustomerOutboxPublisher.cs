using Api.Shared.Clients.Events.Skedular.Customer.V1.Key;
using Api.Shared.Clients.Events.Skedular.Customer.V1.Value;
using Customer.Shared.Mappers;
using Customer.Shared.Workflows.Feedback;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal.Configurations;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Type;

namespace Customer.Shared.Publishers;

public interface ICustomerOutboxPublisher
{
    void PublishCustomers(IEnumerable<Models.Customer> customers, IUnitOfWork unitOfWork);
    void StartWorkflowPayBookingViaBankTransfer(string customerFeedbackId, IUnitOfWork unitOfWork);
}

public class CustomerOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    TemporalConfiguration temporalConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaOutboxEventPublisher<Key, Event> publisher,
    ITemporalOutboxWorkflowExecutor<SubmitCustomerFeedback> temporalOutboxSubmitCustomerFeedbackWorkflowExecutor) : ICustomerOutboxPublisher
{
    public void PublishCustomers(IEnumerable<Models.Customer> customers, IUnitOfWork unitOfWork)
    {
        foreach (var customer in customers)
        {
            publisher.Publish(
                new Key { CustomerId = customer.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        customer.IsDeleted() ? Type.CustomerDeleted : Type.CustomerUpserted,
                        context.GetCorrelationId()),
                    Data = new Data { Customer = mapper.MapTo(customer) }
                },
                unitOfWork);
        }
    }

    public void StartWorkflowPayBookingViaBankTransfer(string customerFeedbackId, IUnitOfWork unitOfWork) =>
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
}
