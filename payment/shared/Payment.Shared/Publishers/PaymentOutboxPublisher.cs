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
    Task PublishOrganizationPaymentMethodStateAsync(
        string organizationId,
        bool hasAttachedPaymentMethod,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);
}

public class PaymentOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IContext context,
    IOutboxEventPublisher<Key, Event> publisher)
    : IPaymentOutboxPublisher
{
    public async Task PublishOrganizationPaymentMethodStateAsync(
        string organizationId,
        bool hasAttachedPaymentMethod,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var key = new Key { OrganizationId = organizationId };
        var @event = new Event
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
        };

        await publisher.PublishAsync(key, @event, unitOfWork, cancellationToken);
    }
}
