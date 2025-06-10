using Api.Shared.Clients.Events.Skedular.Customer.V1.Key;
using Api.Shared.Clients.Events.Skedular.Customer.V1.Value;
using Customer.Shared.Mappers;
using Customer.Shared.Workflows;
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
    void ExecuteWorkflowAddCustomerStripePaymentMethod(AddCustomerStripePaymentMethodInput args, IUnitOfWork unitOfWork);
}

public class CustomerOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaOutboxEventPublisher<Key, Event> publisher,
    TemporalConfiguration temporalConfiguration,
    ITemporalOutboxWorkflowExecutor<AddCustomerStripePaymentMethod, AddCustomerStripePaymentMethodInput>
        temporalOutboxAddCustomerStripePaymentMethodWorkflowExecutor) : ICustomerOutboxPublisher
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
                        customer.IsNotDeleted() ? Type.CustomerUpserted : Type.CustomerDeleted,
                        context.GetCorrelationId()),
                    Data = new Data { Customer = mapper.MapTo(customer) }
                },
                unitOfWork);
        }
    }

    public void ExecuteWorkflowAddCustomerStripePaymentMethod(AddCustomerStripePaymentMethodInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxAddCustomerStripePaymentMethodWorkflowExecutor.Execute(
            new AddCustomerStripePaymentMethodInput(args.CustomerId, args.ClientSecret, args.SetupIntentId),
            new WorkflowOptions
            {
                Id = args.ClientSecret,
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);
}
