using Api.Shared.Clients.Events.Skedular.PaymentInternal.V1.Key;
using Enterprise.Shared.Kafka.Consume;
using Payment.Processors.Mappers;
using Payment.Shared.Publishers;
using Payment.Shared.Repositories;
using Stripe;
using Event = Api.Shared.Clients.Events.Skedular.PaymentInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.PaymentInternal.V1.Value.Type;

namespace Payment.Processors.Subscribers;

public class PaymentInternalSubscriber(IRepositoryFactory repositoryFactory, IMapper mapper, IPaymentPublisher paymentPublisher)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.StripeConnectAccountWebhookEventReceived:
                await HandleStripeConnectAccountWebhookEventReceivedAsync(@event.Data.StripeConnectAccountWebhookEventPayload, cancellationToken);
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleStripeConnectAccountWebhookEventReceivedAsync(string json, CancellationToken cancellationToken)
    {
        var stripeEvent = EventUtility.ParseEvent(json, false);
        ArgumentNullException.ThrowIfNull(stripeEvent);

        switch (stripeEvent.Type)
        {
            case EventTypes.AccountApplicationAuthorized:
                HandleAccountApplicationAuthorized(stripeEvent);
                break;

            case EventTypes.AccountApplicationDeauthorized:
                HandleAccountApplicationDeauthorized(stripeEvent);
                break;

            case EventTypes.AccountExternalAccountCreated:
                HandleAccountExternalAccountCreated(stripeEvent);
                break;

            case EventTypes.AccountExternalAccountDeleted:
                HandleAccountExternalAccountDeleted(stripeEvent);
                break;

            case EventTypes.AccountExternalAccountUpdated:
                HandleAccountExternalAccountUpdated(stripeEvent);
                break;

            case EventTypes.AccountUpdated:
                await HandleAccountUpdatedAsync(stripeEvent, cancellationToken);
                break;
        }
    }

    private void HandleAccountApplicationAuthorized(Stripe.Event stripeEvent)
    {
        var stripeAccount = stripeEvent.Data.Object as Account;
        ArgumentNullException.ThrowIfNull(stripeAccount);
    }

    private void HandleAccountApplicationDeauthorized(Stripe.Event stripeEvent)
    {
        var stripeAccount = stripeEvent.Data.Object as Account;
        ArgumentNullException.ThrowIfNull(stripeAccount);
    }

    private void HandleAccountExternalAccountCreated(Stripe.Event stripeEvent)
    {
        var stripeAccount = stripeEvent.Data.Object as Account;
        ArgumentNullException.ThrowIfNull(stripeAccount);
    }


    private void HandleAccountExternalAccountDeleted(Stripe.Event stripeEvent)
    {
        var stripeAccount = stripeEvent.Data.Object as Account;
        ArgumentNullException.ThrowIfNull(stripeAccount);
    }


    private void HandleAccountExternalAccountUpdated(Stripe.Event stripeEvent)
    {
        var stripeAccount = stripeEvent.Data.Object as Account;
        ArgumentNullException.ThrowIfNull(stripeAccount);
    }


    private async Task HandleAccountUpdatedAsync(Stripe.Event stripeEvent, CancellationToken cancellationToken)
    {
        var stripeAccount = stripeEvent.Data.Object as Account;
        ArgumentNullException.ThrowIfNull(stripeAccount);

        var account = await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByIdAsync(stripeAccount.Id, cancellationToken);
        if (account is null)
        {
            return;
        }

        account = mapper.MergeTo(stripeAccount, account);

        account = repositoryFactory.OrganizationStripeConnectAccountRepository.Update(account);
        var mappedAccount = mapper.MapTo(account);

        await paymentPublisher.PublishOrganizationStripeConnectAccountsAsync([mappedAccount], cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
