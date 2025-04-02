using Api.Shared.Clients.Events.Skedular.Payment.V1.Key;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using Organization.Processors.Mappers;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Event = Api.Shared.Clients.Events.Skedular.Payment.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.Payment.V1.Value.Type;

namespace Organization.Processors.Subscribers;

public class PaymentSubscriber(
    ILogger<PaymentSubscriber> logger,
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider,
    IRandomHelper randomHelper,
    IMapper mapper,
    IOrganizationPublisher organizationPublisher) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.OrganizationPaymentMethodsUpdated:
                await HandleOrganizationPaymentMethodsUpdatedEventAsync(@event, cancellationToken);
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleOrganizationPaymentMethodsUpdatedEventAsync(Event @event, CancellationToken cancellationToken)
    {
        var organizationPaymentMethod = @event.Data.OrganizationPaymentMethod;
        var organization = await repositoryFactory.OrganizationRepository
            .Query(new Specification<Shared.Database.Entities.Organization>
            {
                Criteria = query => query.Id == organizationPaymentMethod.OrganizationId
            }).FirstAsync(cancellationToken);

        var organizationId = organization.Id;

        try
        {
            if (organization.PaymentMethodEventRaisedAt is not null &&
                organization.PaymentMethodEventRaisedAt > @event.Metadata.Time.ToDateTimeOffset())
            {
                logger.LogInformation("Ignoring Payment event. Event timestamp is older that what is already processed.");

                return;
            }

            var attachedPaymentMethodStateChanged = organization.HasAttachedPaymentMethod !=
                                                    organizationPaymentMethod.HasAttachedPaymentMethod;
            if (attachedPaymentMethodStateChanged)
            {
                organization.HasAttachedPaymentMethod = organizationPaymentMethod.HasAttachedPaymentMethod;
                organization = repositoryFactory.OrganizationRepository.Update(organization);
            }

            if (organization.HasAttachedPaymentMethod)
            {
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                return;
            }

            var now = timeProvider.GetUtcNow();
            var organizationOffering = await repositoryFactory.OrganizationOfferingRepository
                .Query(new Specification<OrganizationOffering>
                    {
                        Criteria = query =>
                            !query.DeletedAt.HasValue && query.Organization.Id == organizationId &&
                            query.Start <= now &&
                            query.End >= now
                    }
                    .ApplyOrderBy(query => query.Id))
                .FirstOrDefaultAsync(cancellationToken);
            if (organizationOffering is not null)
            {
                if (organizationOffering.Code.IsFreeOffering())
                {
                    await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

                    return;
                }

                // If current active offering is not free tier, it needs to be deleted
                repositoryFactory.OrganizationOfferingRepository.Remove(organizationOffering);
            }

            // Looking for an existing offering to avoid creating duplicated offering as well as making sure we are not
            // losing track of active users against free offering
            var existingFreeOffering = await repositoryFactory.OrganizationOfferingRepository
                .Query(new Specification<OrganizationOffering>
                    {
                        Criteria = query =>
                            query.Organization.Id == organizationId && query.Start <= now && query.End >= now &&
                            query.Code == OfferingCode.FreeTierV1
                    }
                    .ApplyOrderBy(query => query.Id))
                .FirstOrDefaultAsync(cancellationToken);

            if (existingFreeOffering is null)
            {
                repositoryFactory.OrganizationOfferingRepository.Add(new OrganizationOffering
                {
                    Id = randomHelper.Generate(),
                    Code = OfferingCode.FreeTierV1,
                    Organization = organization,
                    Start = now,
                    End = now.GetOfferingPeriodStart().GetOfferingPeriodEnd(),
                    AutoRenew = true,
                    UnitPrice = OfferingCode.FreeTierV1.GetOffering().UnitPrice
                });
            }
            else
            {
                repositoryFactory.OrganizationOfferingRepository.Undelete(existingFreeOffering);
            }

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            // Always publish latest saved organization state. Since we do not use Outbox here, there is a chance we
            // might have failed to do previous steps and this run is the result of replaying the event from retry topic
            organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
            await organizationPublisher.PublishOrganizationsAsync([mapper.MapTo(organization!)], cancellationToken);
        }
    }
}
