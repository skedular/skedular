using Api.Shared.Clients.Events.Skedular.Payment.V1.Key;
using Api.Shared.Clients.Events.Skedular.Payment.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Outbox.Publishers;
using Payment.Shared.Mappers;
using Event = Api.Shared.Clients.Events.Skedular.Payment.V1.Value.Event;
using StripeConnectAccount = Payment.Shared.Models.StripeConnectAccount;
using Type = Api.Shared.Clients.Events.Skedular.Payment.V1.Value.Type;

namespace Payment.Shared.Publishers;

public interface IPaymentOutboxPublisher
{
    void PublishOrganizationPaymentMethodState(string organizationId, bool hasAttachedPaymentMethod, IUnitOfWork unitOfWork);
    void PublishCustomerPaymentMethodState(string customerId, bool hasAttachedPaymentMethod, IUnitOfWork unitOfWork);
    void PublishOrganizationStripeConnectAccounts(IEnumerable<StripeConnectAccount> accounts, IUnitOfWork unitOfWork);
}

public class PaymentOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaOutboxEventPublisher<Key, Event> publisher)
    : IPaymentOutboxPublisher
{
    public void PublishOrganizationPaymentMethodState(string organizationId, bool hasAttachedPaymentMethod, IUnitOfWork unitOfWork) =>
        publisher.Publish(new Key { OrganizationId = organizationId }, new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.OrganizationPaymentMethodsUpdated,
                    context.GetCorrelationId()),
                Data = new Data
                {
                    OrganizationPaymentMethod = new OrganizationPaymentMethod
                    {
                        OrganizationId = organizationId, HasAttachedPaymentMethod = hasAttachedPaymentMethod
                    }
                }
            },
            unitOfWork);

    public void PublishCustomerPaymentMethodState(string customerId, bool hasAttachedPaymentMethod, IUnitOfWork unitOfWork) =>
        publisher.Publish(new Key { CustomerId = customerId }, new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.CustomerPaymentMethodsUpdated,
                    context.GetCorrelationId()),
                Data = new Data
                {
                    CustomerPaymentMethod = new CustomerPaymentMethod
                    {
                        CustomerId = customerId, HasAttachedPaymentMethod = hasAttachedPaymentMethod
                    }
                }
            },
            unitOfWork);

    public void PublishOrganizationStripeConnectAccounts(IEnumerable<StripeConnectAccount> accounts, IUnitOfWork unitOfWork)
    {
        foreach (var account in accounts)
        {
            publisher.Publish(
                new Key { OrganizationId = account.Id },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        account.IsNotDeleted() ? Type.OrganizationStripeConnectAccountUpserted : Type.OrganizationStripeConnectAccountDeleted,
                        context.GetCorrelationId()),
                    Data = new Data { StripeConnectAccount = mapper.MapTo(account) }
                },
                unitOfWork);
        }
    }
}
