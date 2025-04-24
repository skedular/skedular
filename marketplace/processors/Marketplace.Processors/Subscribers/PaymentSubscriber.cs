using Api.Shared.Clients.Events.Skedular.Payment.V1.Key;
using Enterprise.Shared.Kafka.Consume;
using Marketplace.Processors.Mappers;
using Marketplace.Shared.Database.Entities;
using Marketplace.Shared.Repositories;
using Event = Api.Shared.Clients.Events.Skedular.Payment.V1.Value.Event;
using OrganizationStripeConnectAccount = Marketplace.Shared.Database.Entities.OrganizationStripeConnectAccount;
using Type = Api.Shared.Clients.Events.Skedular.Payment.V1.Value.Type;

namespace Marketplace.Processors.Subscribers;

public class PaymentSubscriber(ILogger<PaymentSubscriber> logger, IMapper mapper, IRepositoryFactory repositoryFactory) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.OrganizationStripeConnectAccountUpserted:
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(@event.Data.OrganizationStripeConnectAccount.OrganizationId);

                    var account = mapper.MapTo(@event);
                    var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(account.Organization.Id, cancellationToken);
                    var existingAccount =
                        await repositoryFactory.OrganizationStripeConnectAccountRepository.UpsertNakedAsync(account.Id, organization,
                            cancellationToken);
                    if (existingAccount.EventRaisedAt > account.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Payment event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleOrganizationStripeConnectAccountUpsertedEventAsync(account, existingAccount, organization, cancellationToken);
                }
                break;

            case Type.OrganizationStripeConnectAccountDeleted:
                {
                    var location = mapper.MapTo(@event);
                    var existingLocation =
                        await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByIdAsync(location.Id, cancellationToken);
                    if (existingLocation is not null && existingLocation.EventRaisedAt > location.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Payment event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    if (existingLocation is null)
                    {
                        return EventSubscriberResults.Success;
                    }

                    await HandleOrganizationStripeConnectAccountDeletedEventAsync(existingLocation, cancellationToken);
                }
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleOrganizationStripeConnectAccountUpsertedEventAsync(
        Shared.Models.OrganizationStripeConnectAccount account,
        OrganizationStripeConnectAccount existingAccount,
        Organization organization,
        CancellationToken cancellationToken)
    {
        _ = repositoryFactory.OrganizationStripeConnectAccountRepository.Update(mapper.MergeToEntity(account, existingAccount, organization));
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleOrganizationStripeConnectAccountDeletedEventAsync(OrganizationStripeConnectAccount existingAccount,
        CancellationToken cancellationToken)
    {
        _ = repositoryFactory.OrganizationStripeConnectAccountRepository.Remove(existingAccount);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
