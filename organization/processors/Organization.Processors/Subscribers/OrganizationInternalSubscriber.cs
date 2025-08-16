using Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Key;
using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;
using Organization.Processors.Mappers;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Stripe;
using Event = Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Value.Type;

namespace Organization.Processors.Subscribers;

public class OrganizationInternalSubscriber(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    IMapper mapper,
    IOrganizationPublisher organizationPublisher)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.RecordDailyMemberCount:
                await HandleRecordDailyMemberCountEventAsync(@event, cancellationToken);
                break;

            case Type.StripeConnectAccountWebhookEventReceived:
                await HandleStripeConnectAccountWebhookEventReceivedAsync(@event.StripeConnectAccountWebhookEventPayload, cancellationToken);
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleRecordDailyMemberCountEventAsync(Event @event, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(@event.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return;
        }

        var startOfToday = timeProvider.GetUtcNow().StartOfDay();
        if (await repositoryFactory.DailyMemberCountRecordingRepository.Query(new Specification<DailyMemberCountRecording>
            {
                Criteria = query => !query.DeletedAt.HasValue && query.Organization.Id == @event.OrganizationId && query.Date == startOfToday
            }).AnyAsync(cancellationToken))
        {
            return;
        }

        _ = repositoryFactory.DailyMemberCountRecordingRepository.Add(new DailyMemberCountRecording
        {
            Id = randomHelper.Generate(),
            Count = organization.OrganizationMembers.Count(item => item.IsNotDeleted()),
            Date = startOfToday,
            Organization = organization
        });

        organization.DailyMemberCountLastRecordedAt = timeProvider.GetUtcNow();
        _ = repositoryFactory.OrganizationRepository.Update(organization);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
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

        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(account.Organization.Id, cancellationToken) ??
                           throw new OrganizationNotFound();
        await organizationPublisher.PublishOrganizationsAsync([mapper.MapTo(organization)], cancellationToken);
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
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(account.Organization.Id, cancellationToken) ??
                           throw new OrganizationNotFound();
        await organizationPublisher.PublishOrganizationsAsync([mapper.MapTo(organization)], cancellationToken);
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

        account = repositoryFactory.OrganizationStripeConnectAccountRepository.Update(mapper.MergeTo(stripeAccount, account));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(account.Organization.Id, cancellationToken) ??
                           throw new OrganizationNotFound();
        await organizationPublisher.PublishOrganizationsAsync([mapper.MapTo(organization)], cancellationToken);
    }
}
