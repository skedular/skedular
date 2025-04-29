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
                await HandleAccountApplicationAuthorizedAsync(stripeEvent, cancellationToken);
                break;

            case EventTypes.AccountApplicationDeauthorized:
                await HandleAccountApplicationDeauthorizedAsync(stripeEvent, cancellationToken);
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

    private async Task HandleAccountApplicationAuthorizedAsync(Stripe.Event stripeEvent, CancellationToken cancellationToken)
    {
        var account =
            await repositoryFactory.StripeConnectAccountRepository.GetByStripeAccountIdAsync(stripeEvent.Account, cancellationToken);
        if (account is null)
        {
            return;
        }

        account.ApplicationAuthorized = true;
        account = repositoryFactory.StripeConnectAccountRepository.Update(account);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await paymentPublisher.PublishOrganizationStripeConnectAccountsAsync([mapper.MapTo(account)], cancellationToken);
    }

    private async Task HandleAccountApplicationDeauthorizedAsync(Stripe.Event stripeEvent, CancellationToken cancellationToken)
    {
        var account =
            await repositoryFactory.StripeConnectAccountRepository.GetByStripeAccountIdAsync(stripeEvent.Account, cancellationToken);
        if (account is null)
        {
            return;
        }

        account.ApplicationAuthorized = false;
        account = repositoryFactory.StripeConnectAccountRepository.Remove(account);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await paymentPublisher.PublishOrganizationStripeConnectAccountsAsync([mapper.MapTo(account)], cancellationToken);
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

        var account =
            await repositoryFactory.StripeConnectAccountRepository.GetByStripeAccountIdAsync(stripeAccount.Id, cancellationToken);
        if (account is null)
        {
            return;
        }

        account = repositoryFactory.StripeConnectAccountRepository.Update(mapper.MergeTo(stripeAccount, account));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await paymentPublisher.PublishOrganizationStripeConnectAccountsAsync([mapper.MapTo(account)], cancellationToken);
    }
}
