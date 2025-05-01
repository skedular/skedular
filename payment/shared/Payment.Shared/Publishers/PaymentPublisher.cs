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
    Task PublishBookingPaymentCreatedAsync(IEnumerable<StripeCheckoutSession> sessions, CancellationToken cancellationToken);
    Task PublishBookingPaymentCompletedAsync(IEnumerable<StripeCheckoutSession> sessions, CancellationToken cancellationToken);
    Task PublishBookingPaymentExpiredAsync(IEnumerable<StripeCheckoutSession> sessions, CancellationToken cancellationToken);
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
                    account.IsNotDeleted() ? Type.OrganizationStripeConnectAccountUpserted : Type.OrganizationStripeConnectAccountDeleted,
                    context.GetCorrelationId()),
                Data = new Data { StripeConnectAccount = mapper.MapTo(account) }
            },
            cancellationToken)));

    public async Task PublishBookingPaymentCreatedAsync(IEnumerable<StripeCheckoutSession> sessions, CancellationToken cancellationToken) =>
        await Task.WhenAll(sessions.Select(session => publisher.PublishAsync(
            new Key { BookingId = session.Booking!.Id },
            new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.BookingPaymentCreated,
                    context.GetCorrelationId()),
                Data = new Data { BookingPaymentCreated = mapper.MapToBookingPaymentCreatedDetails(session) }
            },
            cancellationToken)));

    public async Task PublishBookingPaymentCompletedAsync(IEnumerable<StripeCheckoutSession> sessions, CancellationToken cancellationToken) =>
        await Task.WhenAll(sessions.Select(session => publisher.PublishAsync(
            new Key { BookingId = session.Booking!.Id },
            new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.BookingPaymentCompleted,
                    context.GetCorrelationId()),
                Data = new Data { BookingPayment = mapper.MapToBookingPaymentDetails(session) }
            },
            cancellationToken)));

    public async Task PublishBookingPaymentExpiredAsync(IEnumerable<StripeCheckoutSession> sessions, CancellationToken cancellationToken) =>
        await Task.WhenAll(sessions.Select(session => publisher.PublishAsync(
            new Key { BookingId = session.Booking!.Id },
            new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.BookingPaymentExpired,
                    context.GetCorrelationId()),
                Data = new Data { BookingPayment = mapper.MapToBookingPaymentDetails(session) }
            },
            cancellationToken)));
}
