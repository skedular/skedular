using Api.Shared.Clients.Events.Skedular.BillingInternal.V1.Key;
using Billing.Processors.Mappers;
using Billing.Shared.Database.Entities;
using Billing.Shared.Publishers;
using Billing.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using Microsoft.EntityFrameworkCore;
using Event = Api.Shared.Clients.Events.Skedular.BillingInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.BillingInternal.V1.Value.Type;

namespace Billing.Processors.Subscribers;

public class BillingInternalSubscriber(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider,
    IMapper mapper,
    IBillingOutboxPublisher billingOutboxPublisher)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.GenerateOrganizationOfferingInvoice:
                await HandleGenerateOrganizationOfferingInvoiceEventAsync(@event.OrganizationOfferingId, cancellationToken);
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleGenerateOrganizationOfferingInvoiceEventAsync(string organizationOfferingId, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var organizationOffering = await repositoryFactory.OrganizationOfferingRepository
            .Query(new Specification<OrganizationOffering>
                {
                    Criteria = query => query.Id == organizationOfferingId && query.End <= now && !query.InvoiceDate.HasValue
                }
                .AddInclude(query => query.Organization))
            .FirstOrDefaultAsync(cancellationToken);
        if (organizationOffering is null)
        {
            return;
        }

        organizationOffering.TotalCost = organizationOffering.TotalNumberOfActiveCustomers * organizationOffering.UnitPrice;
        organizationOffering.InvoiceDate = now;

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.OrganizationOfferingRepository.Update(organizationOffering);
        await billingOutboxPublisher.PublishBillingOrganizationsOfferingsAsync(
            [mapper.MapTo(organizationOffering)],
            repositoryFactory.UnitOfWork,
            cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
