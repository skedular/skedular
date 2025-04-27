using Api.Shared.Clients.Events.Skedular.Booking.V1.Key;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Random;
using Enterprise.Shared.Sanitization;
using Microsoft.EntityFrameworkCore;
using Organization.Processors.Mappers;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Event = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event;
using Booking = Organization.Shared.Database.Entities.Booking;
using Type = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Type;

namespace Organization.Processors.Subscribers;

public class BookingSubscriber(
    ILogger<BookingSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IOrganizationPublisher organizationPublisher) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.BookingUpserted:
                {
                    var booking = mapper.MapTo(@event);
                    if (!booking.InvolvedOrganizations.Select(item => item.Id).RemoveInvalidIds()!.Any())
                    {
                        await HandleBookingDeletedEventAsync(booking, cancellationToken);
                    }
                    else
                    {
                        var existingBooking = await repositoryFactory.BookingRepository.UpsertNakedAsync(booking.Id, cancellationToken);
                        if (existingBooking.EventRaisedAt > booking.EventRaisedAt)
                        {
                            logger.LogInformation("Ignoring Booking event. Event timestamp is older that what is already processed.");

                            return EventSubscriberResults.Success;
                        }

                        await TrackActiveMembersAsync(@event, cancellationToken);
                        await HandleBookingUpsertedEventAsync(booking, existingBooking, cancellationToken);
                    }
                }
                break;

            case Type.BookingDeleted:
                {
                    var booking = mapper.MapTo(@event);
                    await HandleBookingDeletedEventAsync(booking, cancellationToken);
                }
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task TrackActiveMembersAsync(Event @event, CancellationToken cancellationToken)
    {
        var customerId = @event.Data.Booking.CustomerId;
        var organizationId = @event.Data.Booking.OrganizationId;

        if (string.IsNullOrWhiteSpace(customerId))
        {
            // The booking is not attached to any customer for whatever reason, ignoring it
            return;
        }

        if (string.IsNullOrWhiteSpace(organizationId))
        {
            // Booking not attached to organization, ignoring it
            return;
        }

        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            // Organization not found, ignoring it
            return;
        }

        var organizationOffering = organization.OrganizationOfferings.SingleOrDefault();
        if (organizationOffering is null)
        {
            // Organization offering does not exist for whatever reason, ignoring it
            return;
        }

        var organizationMember = organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId);
        if (organizationMember is null)
        {
            // Customer isn't found ignoring it
            return;
        }

        var organizationOfferingActiveMember = await repositoryFactory.OrganizationOfferingActiveMemberRepository.Query(
            new Specification<OrganizationOfferingActiveMember>
            {
                Criteria = query =>
                    query.OrganizationOffering.Id == organizationOffering.Id && query.OrganizationMember.Id == organizationMember.Id
            }.ApplyOrderBy(query => query.Id)).FirstOrDefaultAsync(cancellationToken);

        _ = organizationOfferingActiveMember is null
            ? repositoryFactory.OrganizationOfferingActiveMemberRepository.Add(
                new OrganizationOfferingActiveMember
                {
                    Id = randomHelper.Generate(), OrganizationMember = organizationMember, OrganizationOffering = organizationOffering
                })
            : repositoryFactory.OrganizationOfferingActiveMemberRepository.Update(organizationOfferingActiveMember);

        if (organizationOfferingActiveMember is null)
        {
            await organizationPublisher.PublishOrganizationsAsync([mapper.MapTo(organization)], cancellationToken);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleBookingUpsertedEventAsync(Shared.Models.Booking booking, Booking existingBooking, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(booking.Organization.Id, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        var involvedOrganizations = await repositoryFactory.OrganizationRepository.GetByIdsAsync(
            booking.InvolvedOrganizations.Select(item => item.Id).ToList(),
            cancellationToken);
        _ = repositoryFactory.BookingRepository.Update(mapper.MergeToEntity(booking, existingBooking, organization, involvedOrganizations));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleBookingDeletedEventAsync(Shared.Models.Booking booking, CancellationToken cancellationToken)
    {
        var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(booking.Id, cancellationToken);
        if (existingBooking is not null && existingBooking.EventRaisedAt > booking.EventRaisedAt)
        {
            logger.LogInformation("Ignoring Booking event. Event timestamp is older that what is already processed.");

            return;
        }

        if (existingBooking is null)
        {
            return;
        }


        _ = repositoryFactory.BookingRepository.Remove(existingBooking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
