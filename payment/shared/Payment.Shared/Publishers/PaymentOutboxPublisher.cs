using Api.Shared.Clients.Events.Skedular.Payment.V1.Key;
using Api.Shared.Clients.Events.Skedular.Payment.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Event = Api.Shared.Clients.Events.Skedular.Payment.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Payment.V1.Value.Type;

namespace Payment.Shared.Publishers;

public interface IPaymentOutboxPublisher
{
    void PublishOrganizationPaymentMethodState(string organizationId, bool hasAttachedPaymentMethod, IUnitOfWork unitOfWork);
}

public class PaymentOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IContext context,
    IOutboxEventPublisher<Key, Event> publisher)
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
}
