using Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1;
using Api.Shared.Services;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Random;
using Organization.Processors.Mappers;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Stripe;
using Event = Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Event;
using Type = Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Type;

namespace Organization.Processors.Subscribers;

public class OrganizationInternalSubscriber(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IEventMapper eventMapper,
    IOrganizationPublisher organizationPublisher)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.StripeConnectAccountWebhookEventReceived:
                await HandleStripeConnectAccountWebhookEventReceivedAsync(@event.StripeConnectAccountWebhookEventPayload, cancellationToken);
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
                break;

            case EventTypes.AccountExternalAccountDeleted:
                break;

            case EventTypes.AccountExternalAccountUpdated:
                break;

            case EventTypes.AccountUpdated:
                await HandleAccountUpdatedAsync(stripeEvent, cancellationToken);
                break;
        }
    }

    private async Task HandleAccountApplicationAuthorizedAsync(Stripe.Event stripeEvent, CancellationToken cancellationToken)
    {
        var account =
            await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByStripeAccountIdAsync(stripeEvent.Account, cancellationToken);
        if (account is null)
        {
            return;
        }

        if (account.OrganizationStripeConnectAccountAuthorization is null)
        {
            account.OrganizationStripeConnectAccountAuthorization = repositoryFactory.OrganizationStripeConnectAccountAuthorizationRepository.Add(
                new OrganizationStripeConnectAccountAuthorization { Id = randomHelper.Generate(), IsAuthorized = true });
        }
        else
        {
            account.OrganizationStripeConnectAccountAuthorization.IsAuthorized = true;
            repositoryFactory.OrganizationStripeConnectAccountAuthorizationRepository.Update(account.OrganizationStripeConnectAccountAuthorization);
        }

        account = repositoryFactory.OrganizationStripeConnectAccountRepository.Update(account);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               account.Organization.Id,
                               null,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        await organizationPublisher.PublishOrganizationsAsync([eventMapper.MapTo(organization)], cancellationToken);
    }

    private async Task HandleAccountApplicationDeauthorizedAsync(Stripe.Event stripeEvent, CancellationToken cancellationToken)
    {
        var account =
            await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByStripeAccountIdAsync(stripeEvent.Account, cancellationToken);
        if (account is null)
        {
            return;
        }

        if (account.OrganizationStripeConnectAccountAuthorization is null)
        {
            account.OrganizationStripeConnectAccountAuthorization = repositoryFactory.OrganizationStripeConnectAccountAuthorizationRepository.Add(
                new OrganizationStripeConnectAccountAuthorization { Id = randomHelper.Generate(), IsAuthorized = false });
        }
        else
        {
            account.OrganizationStripeConnectAccountAuthorization.IsAuthorized = false;
            repositoryFactory.OrganizationStripeConnectAccountAuthorizationRepository.Update(account.OrganizationStripeConnectAccountAuthorization);
        }

        account = repositoryFactory.OrganizationStripeConnectAccountRepository.Remove(account);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               account.Organization.Id,
                               null,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        await organizationPublisher.PublishOrganizationsAsync([eventMapper.MapTo(organization)], cancellationToken);
    }

    private async Task HandleAccountUpdatedAsync(Stripe.Event stripeEvent, CancellationToken cancellationToken)
    {
        var stripeAccount = stripeEvent.Data.Object as Account;
        ArgumentNullException.ThrowIfNull(stripeAccount);

        var account =
            await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByStripeAccountIdAsync(stripeAccount.Id, cancellationToken);
        if (account is null)
        {
            return;
        }

        account = repositoryFactory.OrganizationStripeConnectAccountRepository.Update(eventMapper.MergeTo(stripeAccount, account));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               account.Organization.Id,
                               null,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        await organizationPublisher.PublishOrganizationsAsync([eventMapper.MapTo(organization)], cancellationToken);
    }
}
