using Api.Shared.Clients.Events.Skedular.Payment.V1.Key;
using Api.Shared.Clients.Events.Skedular.Payment.V1.Value;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Enterprise.Shared.Models;
using Payment.Shared.Mappers;
using Event = Api.Shared.Clients.Events.Skedular.Payment.V1.Value.Event;
using StripeCheckoutSession = Payment.Shared.Models.StripeCheckoutSession;
using StripeConnectAccount = Payment.Shared.Models.StripeConnectAccount;
using Type = Api.Shared.Clients.Events.Skedular.Payment.V1.Value.Type;

namespace Payment.Shared.Publishers;

public interface IPaymentPublisher
{
    Task PublishOrganizationStripeConnectAccountsAsync(IEnumerable<StripeConnectAccount> accounts, CancellationToken cancellationToken);
    Task PublishBookingCheckoutSessionAsync(IEnumerable<StripeCheckoutSession> sessions, CancellationToken cancellationToken);
}

public class PaymentPublisher(
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : IPaymentPublisher
{
    public async Task PublishOrganizationStripeConnectAccountsAsync(
        IEnumerable<StripeConnectAccount> accounts,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(accounts.Select(account => publisher.PublishAsync(
            new Key { OrganizationId = account.Id },
            new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    account.IsDeleted() ? Type.OrganizationStripeConnectAccountDeleted : Type.OrganizationStripeConnectAccountUpserted,
                    context.GetCorrelationId()),
                Data = new Data { StripeConnectAccount = mapper.MapTo(account) }
            },
            cancellationToken)));

    public async Task PublishBookingCheckoutSessionAsync(IEnumerable<StripeCheckoutSession> sessions, CancellationToken cancellationToken) =>
        await Task.WhenAll(sessions.Select(session => publisher.PublishAsync(
            new Key { BookingCheckoutSessionId = session.Booking!.Id },
            new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    session.IsDeleted() ? Type.BookingCheckoutSessionDeleted : Type.BookingCheckoutSessionUpserted,
                    context.GetCorrelationId()),
                Data = new Data { BookingCheckoutSession = mapper.MapTo(session) }
            },
            cancellationToken)));
}
