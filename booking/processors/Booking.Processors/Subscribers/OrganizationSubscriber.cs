using Api.Shared.Clients.Events.UnityHub.Organization.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Organization.V1.Value;
using Booking.Processors.Mappers;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Kafka.Consume;
using Organization = Booking.Shared.Database.Entities.Organization;
using Type = Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Type;

namespace Booking.Processors.Subscribers;

public class OrganizationSubscriber(
    ILogger<OrganizationSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(
        EventContext eventContext,
        Key key,
        Event @event,
        CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.OrganizationUpserted:
                {
                    var organization = mapper.MapTo(@event);
                    var existingOrganization =
                        await repositoryFactory.OrganizationRepository.GetByIdAsync(organization.Id, cancellationToken);
                    if (existingOrganization is not null &&
                        existingOrganization.EventRaisedAt > organization.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Organization event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleOrganizationUpsertedEventAsync(organization, existingOrganization, cancellationToken);
                }
                break;

            case Type.OrganizationDeleted:
                {
                    var organization = mapper.MapTo(@event);
                    var existingOrganization =
                        await repositoryFactory.OrganizationRepository.GetByIdAsync(organization.Id, cancellationToken);
                    if (existingOrganization is not null &&
                        existingOrganization.EventRaisedAt > organization.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Organization event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    if (existingOrganization is null)
                    {
                        return EventSubscriberResults.Success;
                    }

                    await HandleOrganizationDeletedEventAsync(existingOrganization, cancellationToken);
                }
                break;

            case Type.InvitationToJoinOrganizationUpserted:
            case Type.InvitationToJoinOrganizationDeleted:
            case Type.OrganizationOfferingUpdated:
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleOrganizationUpsertedEventAsync(
        Shared.Models.Organization organization,
        Organization? existingOrganization,
        CancellationToken cancellationToken)
    {
        existingOrganization = existingOrganization is null
            ? repositoryFactory.OrganizationRepository.Add(mapper.MapToEntity(organization))
            : repositoryFactory.OrganizationRepository.Update(mapper.MergeToEntity(organization,
                existingOrganization));

        existingOrganization = RebuildOrganizationTags(organization, existingOrganization);
        _ = await RebuildOrganizationMembersAsync(organization, existingOrganization, cancellationToken);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleOrganizationDeletedEventAsync(
        Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        repositoryFactory.OrganizationMemberRepository.RemoveRange(existingOrganization.OrganizationMembers);
        _ = repositoryFactory.OrganizationRepository.Remove(existingOrganization);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Organization> RebuildOrganizationMembersAsync(
        Shared.Models.Organization organization,
        Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        var itemsToRemove = existingOrganization.OrganizationMembers
            .Where(organizationMember => organization.OrganizationMembers.All(item => item.Id != organizationMember.Id))
            .ToList();
        var updatedItems = new List<OrganizationMember>();
        foreach (var organizationMember in existingOrganization.OrganizationMembers.Where(organizationMember =>
                     organization.OrganizationMembers.Any(item => item.Id == organizationMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(
                    organizationMember.Customer.Id,
                    cancellationToken);
            updatedItems.Add(repositoryFactory.OrganizationMemberRepository.Update(
                mapper.MergeToEntity(
                    organization.OrganizationMembers.Single(item => item.Id == organizationMember.Id),
                    organizationMember,
                    existingOrganization,
                    customer)));
        }

        var addedItems = new List<OrganizationMember>();
        foreach (var organizationMember in organization.OrganizationMembers
                     .Where(organizationMember =>
                         existingOrganization.OrganizationMembers.All(item => item.Id != organizationMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(organizationMember.Customer.Id,
                    cancellationToken);
            addedItems.Add(
                repositoryFactory.OrganizationMemberRepository.Add(
                    mapper.MapToEntity(organizationMember, existingOrganization, customer)));
        }

        repositoryFactory.OrganizationMemberRepository.RemoveRange(itemsToRemove);
        existingOrganization.OrganizationMembers = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        return existingOrganization;
    }

    private Organization RebuildOrganizationTags(
        Shared.Models.Organization organization,
        Organization existingOrganization)
    {
        var itemsToRemove = existingOrganization.Tags
            .Where(tag => organization.Tags.All(item => item.Id != tag.Id)).ToList();
        var updatedItems = existingOrganization.Tags
            .Where(organizationTag => organization.Tags.Any(item => item.Id == organizationTag.Id))
            .Select(organizationTag => repositoryFactory.OrganizationTagRepository.Update(
                mapper.MergeToEntity(
                    organization.Tags.Single(item => item.Id == organizationTag.Id),
                    organizationTag,
                    existingOrganization)))
            .ToList();
        var addedItems = organization.Tags
            .Where(organizationTag => existingOrganization.Tags.All(item => item.Id != organizationTag.Id))
            .Select(organizationTag =>
                repositoryFactory.OrganizationTagRepository.Add(
                    mapper.MapToEntity(organizationTag, existingOrganization)))
            .ToList();

        repositoryFactory.OrganizationTagRepository.RemoveRange(itemsToRemove);
        existingOrganization.Tags = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        return existingOrganization;
    }
}
