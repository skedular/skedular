using Api.Shared.Clients.Events.UnityHub.OrganizationInternal.V1.Key;
using Api.Shared.Services.Offering;
using Confluent.Kafka;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;
using Organization.Processors.Mappers;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Event = Api.Shared.Clients.Events.UnityHub.OrganizationInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.OrganizationInternal.V1.Value.Type;

namespace Organization.Processors.Subscribers;

public class OrganizationInternalSubscriber(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    IMapper mapper,
    IOrganizationOutboxPublisher organizationOutboxPublisher)
    : IEventSubscriber<Key, Event>
{
    public async Task HandleAsync(Headers headers, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.RenewOrganizationOffering:
                await HandleRenewOrganizationOfferingEventAsync(@event, cancellationToken);
                break;

            case Type.RecordDailyMemberCount:
                await HandleRecordDailyMemberCountEventAsync(@event, cancellationToken);
                break;

            default:
                return;
        }
    }

    private async Task HandleRenewOrganizationOfferingEventAsync(Event @event, CancellationToken cancellationToken)
    {
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(@event.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return;
        }

        var now = timeProvider.GetUtcNow();
        var expiredOfferingsRequireAutoRenew = await repositoryFactory.OrganizationOfferingRepository
            .Query(new Specification<OrganizationOffering>
            {
                Criteria = query =>
                    !query.DeletedAt.HasValue && query.Organization.Id == @event.OrganizationId && query.End <= now &&
                    query.AutoRenew
            }.ApplyOrderByDescending(query => query.End))
            .ToListAsync(cancellationToken);

        if (expiredOfferingsRequireAutoRenew.Count == 0)
        {
            return;
        }

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.OrganizationOfferingRepository.UnitOfWork,
                cancellationToken);

        var expiredOfferingRequireAutoRenew = expiredOfferingsRequireAutoRenew.First();
        var offering = expiredOfferingRequireAutoRenew.Code.GetOffering();
        var start = expiredOfferingRequireAutoRenew.End.GetNextOfferingPeriodStart();

        _ = repositoryFactory.OrganizationOfferingRepository.Add(new OrganizationOffering
        {
            Id = randomHelper.Generate(),
            Code = expiredOfferingRequireAutoRenew.Code,
            Start = start,
            End = start.GetOfferingPeriodEnd(),
            AutoRenew = expiredOfferingRequireAutoRenew.AutoRenew,
            UnitPrice = offering.UnitPrice,
            Organization = organization
        });
        repositoryFactory.OrganizationOfferingRepository.RemoveRange(expiredOfferingsRequireAutoRenew);

        var mappedOrganization = mapper.MapTo(organization);
        mappedOrganization.OrganizationOfferings =
        [
            mappedOrganization.OrganizationOfferings.Where(item => !item.DeletedAt.HasValue)
                .OrderByDescending(item => item.End).First()
        ];

        await organizationOutboxPublisher.PublishOrganizationAsync(
            [mappedOrganization],
            repositoryFactory.OrganizationOfferingRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.OrganizationOfferingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task HandleRecordDailyMemberCountEventAsync(Event @event, CancellationToken cancellationToken)
    {
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(@event.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return;
        }

        var startOfToday = timeProvider.GetUtcNow().StartOfDay();
        if (await repositoryFactory.DailyMemberCountRecordingRepository
                .Query(new Specification<DailyMemberCountRecording>
                {
                    Criteria = query =>
                        !query.DeletedAt.HasValue && query.Organization.Id == @event.OrganizationId &&
                        query.Date == startOfToday
                }).AnyAsync(cancellationToken))
        {
            return;
        }

        _ = repositoryFactory.DailyMemberCountRecordingRepository.Add(new DailyMemberCountRecording
        {
            Id = randomHelper.Generate(),
            Count = organization.OrganizationMembers.Count(item => item.DeletedAt is null),
            Date = startOfToday,
            Organization = organization
        });

        organization.DailyMemberCountLastRecordedAt = timeProvider.GetUtcNow();
        _ = repositoryFactory.OrganizationRepository.Update(organization);

        await repositoryFactory.DailyMemberCountRecordingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
