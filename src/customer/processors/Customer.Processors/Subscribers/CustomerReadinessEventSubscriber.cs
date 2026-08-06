using Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1;
using Customer.Shared.Models;
using Customer.Shared.Repositories;
using Customer.Shared.Services.Cache;
using Enterprise.Shared.Kafka.Consume;
using Domain = Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1.Domain;
using Type = Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1.Type;

namespace Customer.Processors.Subscribers;

public class CustomerReadinessEventSubscriber(
    ILogger<CustomerReadinessEventSubscriber> logger,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.CustomerIdentityProvisioned:
                {
                    var customerId = @event.Data.CustomerIdentityProvisioned.CustomerId;
                    var domain = MapDomainToString(@event.Data.CustomerIdentityProvisioned.Domain);

                    if (domain is null)
                    {
                        logger.LogWarning(
                            "Skipping CustomerIdentityProvisioned for customer {CustomerId}: unmappable domain value {DomainValue}",
                            customerId,
                            @event.Data.CustomerIdentityProvisioned.Domain);

                        return EventSubscriberResults.Success;
                    }

                    logger.LogInformation("Marking domain {Domain} as provisioned for customer {CustomerId}", domain, customerId);

                    await repositoryFactory.CustomerRepository.MarkDomainProvisionedAsync(customerId, domain, cancellationToken);
                    await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                    await cachedCustomerService.RemoveByIdAsync(customerId, cancellationToken);

                    logger.LogInformation("Domain {Domain} provisioned successfully for customer {CustomerId}", domain, customerId);
                }
                break;

            default:
                logger.LogWarning("Unhandled CustomerReadiness event type {EventType}", @event.Metadata.Type);
                break;
        }

        return EventSubscriberResults.Success;
    }

    private static string? MapDomainToString(Domain domain) => domain switch
    {
        Domain.Booking => CustomerReadinessState.Domains.Booking,
        Domain.Organization => CustomerReadinessState.Domains.Organization,
        Domain.Team => CustomerReadinessState.Domains.Team,
        Domain.Marketplace => CustomerReadinessState.Domains.Marketplace,
        Domain.Location => CustomerReadinessState.Domains.Location,
        Domain.Core => CustomerReadinessState.Domains.Core,
        Domain.Slack => CustomerReadinessState.Domains.Slack,
        Domain.MsTeams => CustomerReadinessState.Domains.MsTeams,
        _ => null,
    };
}
