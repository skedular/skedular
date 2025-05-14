using Api.Shared.Clients.Events.Skedular.PaymentInternal.V1.Key;
using Api.Shared.Services.Models;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Random;
using Payment.Processors.Mappers;
using Payment.Shared.Database.Entities;
using Payment.Shared.Publishers;
using Payment.Shared.Repositories;
using Stripe;
using Stripe.Checkout;
using Event = Api.Shared.Clients.Events.Skedular.PaymentInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.PaymentInternal.V1.Value.Type;

namespace Payment.Processors.Subscribers;

public class PaymentInternalSubscriber(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IPaymentPublisher paymentPublisher,
    IRandomHelper randomHelper)
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

            case EventTypes.CheckoutSessionCompleted:
                await HandleCheckoutSessionCompletedAsync(stripeEvent, cancellationToken);
                break;

            case EventTypes.CheckoutSessionExpired:
                await HandleCheckoutSessionExpiredAsync(stripeEvent, cancellationToken);
                break;
        }
    }

    private async Task HandleAccountApplicationAuthorizedAsync(Stripe.Event stripeEvent, CancellationToken cancellationToken)
    {
        var account = await repositoryFactory.StripeConnectAccountRepository.GetByStripeAccountIdAsync(stripeEvent.Account, cancellationToken);
        if (account is null)
        {
            return;
        }

        if (account.StripeConnectAccountAuthorization is null)
        {
            account.StripeConnectAccountAuthorization = repositoryFactory.StripeConnectAccountAuthorizationRepository.Add(
                new StripeConnectAccountAuthorization { Id = randomHelper.Generate(), IsAuthorized = true });
        }
        else
        {
            account.StripeConnectAccountAuthorization.IsAuthorized = true;
            repositoryFactory.StripeConnectAccountAuthorizationRepository.Update(account.StripeConnectAccountAuthorization);
        }

        account = repositoryFactory.StripeConnectAccountRepository.Update(account);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await paymentPublisher.PublishOrganizationStripeConnectAccountsAsync([mapper.MapTo(account)], cancellationToken);
    }

    private async Task HandleAccountApplicationDeauthorizedAsync(Stripe.Event stripeEvent, CancellationToken cancellationToken)
    {
        var account = await repositoryFactory.StripeConnectAccountRepository.GetByStripeAccountIdAsync(stripeEvent.Account, cancellationToken);
        if (account is null)
        {
            return;
        }

        if (account.StripeConnectAccountAuthorization is null)
        {
            account.StripeConnectAccountAuthorization = repositoryFactory.StripeConnectAccountAuthorizationRepository.Add(
                new StripeConnectAccountAuthorization { Id = randomHelper.Generate(), IsAuthorized = false });
        }
        else
        {
            account.StripeConnectAccountAuthorization.IsAuthorized = false;
            repositoryFactory.StripeConnectAccountAuthorizationRepository.Update(account.StripeConnectAccountAuthorization);
        }

        account = repositoryFactory.StripeConnectAccountRepository.Remove(account);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await paymentPublisher.PublishOrganizationStripeConnectAccountsAsync([mapper.MapTo(account)], cancellationToken);
    }

    private void HandleAccountExternalAccountCreated(Stripe.Event stripeEvent)
    {
    }


    private void HandleAccountExternalAccountDeleted(Stripe.Event stripeEvent)
    {
    }

    private void HandleAccountExternalAccountUpdated(Stripe.Event stripeEvent)
    {
    }

    private async Task HandleAccountUpdatedAsync(Stripe.Event stripeEvent, CancellationToken cancellationToken)
    {
        var stripeAccount = stripeEvent.Data.Object as Account;
        ArgumentNullException.ThrowIfNull(stripeAccount);

        var account = await repositoryFactory.StripeConnectAccountRepository.GetByStripeAccountIdAsync(stripeAccount.Id, cancellationToken);
        if (account is null)
        {
            return;
        }

        account = repositoryFactory.StripeConnectAccountRepository.Update(mapper.MergeTo(stripeAccount, account));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await paymentPublisher.PublishOrganizationStripeConnectAccountsAsync([mapper.MapTo(account)], cancellationToken);
    }

    private async Task HandleCheckoutSessionCompletedAsync(Stripe.Event stripeEvent, CancellationToken cancellationToken)
    {
        var session = stripeEvent.Data.Object as Session;
        ArgumentNullException.ThrowIfNull(session);

        var stripeCheckoutSession =
            await repositoryFactory.StripeCheckoutSessionRepository.GetByStripeCheckoutSessionIdAsync(session.Id, cancellationToken);
        if (stripeCheckoutSession is null)
        {
            return;
        }

        stripeCheckoutSession = repositoryFactory.StripeCheckoutSessionRepository.Update(mapper.MergeTo(session, stripeCheckoutSession));
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await paymentPublisher.PublishBookingCheckoutSessionAsync([mapper.MapTo(stripeCheckoutSession)], cancellationToken);
    }

    private async Task HandleCheckoutSessionExpiredAsync(Stripe.Event stripeEvent, CancellationToken cancellationToken)
    {
        var session = stripeEvent.Data.Object as Session;
        ArgumentNullException.ThrowIfNull(session);

        var stripeCheckoutSession =
            await repositoryFactory.StripeCheckoutSessionRepository.GetByStripeCheckoutSessionIdAsync(session.Id, cancellationToken);
        if (stripeCheckoutSession is null)
        {
            return;
        }

        stripeCheckoutSession = mapper.MergeTo(session, stripeCheckoutSession);
        stripeCheckoutSession.PaymentStatus = PaymentStatusConstants.Expired;
        stripeCheckoutSession = repositoryFactory.StripeCheckoutSessionRepository.Update(stripeCheckoutSession);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await paymentPublisher.PublishBookingCheckoutSessionAsync([mapper.MapTo(stripeCheckoutSession)], cancellationToken);
    }
}
