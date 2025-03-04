using Api.Shared.Clients.Events.Skedular.Organization.V1.Key;
using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Enterprise.Shared.Kafka.Consume;
using Location.Processors.Mappers;
using Location.Shared.Repositories;
using Organization = Location.Shared.Database.Entities.Organization;
using OrganizationMember = Location.Shared.Database.Entities.OrganizationMember;
using Type = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Type;

namespace Location.Processors.Subscribers;

public class OrganizationSubscriber(
    ILogger<OrganizationSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.OrganizationUpserted:
                {
                    var organization = mapper.MapTo(@event);
                    var existingOrganization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organization.Id, cancellationToken);
                    if (existingOrganization.EventRaisedAt > organization.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Organization event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleOrganizationUpsertedEventAsync(organization, existingOrganization, cancellationToken);
                }
                break;

            case Type.OrganizationDeleted:
                {
                    var organization = mapper.MapTo(@event);
                    var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdAsync(
                        organization.Id,
                        true,
                        true,
                        true,
                        cancellationToken);
                    if (existingOrganization is not null && existingOrganization.EventRaisedAt > organization.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Organization event. Event timestamp is older that what is already processed.");

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
            : repositoryFactory.OrganizationRepository.Update(mapper.MergeToEntity(organization, existingOrganization));

        existingOrganization = RebuildOrganizationTags(organization, existingOrganization);
        existingOrganization = RebuildOrganizationResourceTypes(organization, existingOrganization);
        _ = await RebuildOrganizationMembersAsync(organization, existingOrganization, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleOrganizationDeletedEventAsync(Organization existingOrganization, CancellationToken cancellationToken)
    {
        repositoryFactory.OrganizationMemberRepository.RemoveRange(existingOrganization.OrganizationMembers);
        _ = repositoryFactory.OrganizationRepository.Remove(existingOrganization);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Organization> RebuildOrganizationMembersAsync(
        Shared.Models.Organization organization,
        Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        var organizationMembers = await repositoryFactory.OrganizationMemberRepository.GetByOrganizationIdAsync(
            existingOrganization.Id,
            cancellationToken);
        var itemsToRemove = organizationMembers
            .Where(organizationMember => organization.OrganizationMembers.All(item => item.Id != organizationMember.Id))
            .ToList();
        var updatedItems = new List<OrganizationMember>();
        foreach (var organizationMember in organizationMembers
                     .Where(organizationMember => organization.OrganizationMembers.Any(item => item.Id == organizationMember.Id)))
        {
            var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(organizationMember.Customer.Id, cancellationToken);
            var updatedOrganizationMember = mapper.MergeToEntity(
                organization.OrganizationMembers.First(item => item.Id == organizationMember.Id),
                organizationMember,
                existingOrganization,
                customer);
            updatedOrganizationMember.DeletedAt = null;
            updatedItems.Add(repositoryFactory.OrganizationMemberRepository.Update(updatedOrganizationMember));
        }

        var addedItems = new List<OrganizationMember>();
        foreach (var organizationMember in organization.OrganizationMembers
                     .Where(organizationMember => organizationMembers.All(item => item.Id != organizationMember.Id)))
        {
            var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(organizationMember.Customer.Id, cancellationToken);
            addedItems.Add(
                repositoryFactory.OrganizationMemberRepository.Add(
                    mapper.MapToEntity(organizationMember, existingOrganization, customer)));
        }

        repositoryFactory.OrganizationMemberRepository.RemoveRange(itemsToRemove);
        existingOrganization.OrganizationMembers = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        return existingOrganization;
    }

    private Organization RebuildOrganizationTags(Shared.Models.Organization organization, Organization existingOrganization)
    {
        var itemsToRemove = existingOrganization.Tags.Where(tag => organization.Tags.All(item => item.Id != tag.Id)).ToList();
        var updatedItems = existingOrganization.Tags
            .Where(organizationTag => organization.Tags.Any(item => item.Id == organizationTag.Id))
            .Select(organizationTag =>
            {
                var updatedOrganizationTag = mapper.MergeToEntity(
                    organization.Tags.First(item => item.Id == organizationTag.Id),
                    organizationTag,
                    existingOrganization);

                updatedOrganizationTag.DeletedAt = null;
                return repositoryFactory.OrganizationTagRepository.Update(updatedOrganizationTag);
            })
            .ToList();
        var addedItems = organization.Tags
            .Where(organizationTag => existingOrganization.Tags.All(item => item.Id != organizationTag.Id))
            .Select(organizationTag => repositoryFactory.OrganizationTagRepository.Add(mapper.MapToEntity(organizationTag, existingOrganization)))
            .ToList();

        repositoryFactory.OrganizationTagRepository.RemoveRange(itemsToRemove);
        existingOrganization.Tags = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        return existingOrganization;
    }

    private Organization RebuildOrganizationResourceTypes(Shared.Models.Organization organization, Organization existingOrganization)
    {
        var itemsToRemove = existingOrganization.ResourceTypes.Where(tag => organization.ResourceTypes.All(item => item.Id != tag.Id)).ToList();
        var updatedItems = existingOrganization.ResourceTypes
            .Where(organizationResourceType => organization.ResourceTypes.Any(item => item.Id == organizationResourceType.Id))
            .Select(organizationResourceType =>
            {
                var updatedOrganizationResourceType = mapper.MergeToEntity(
                    organization.ResourceTypes.First(item => item.Id == organizationResourceType.Id),
                    organizationResourceType,
                    existingOrganization);

                updatedOrganizationResourceType.DeletedAt = null;
                return repositoryFactory.OrganizationResourceTypeRepository.Update(updatedOrganizationResourceType);
            })
            .ToList();
        var addedItems = organization.ResourceTypes
            .Where(organizationResourceType => existingOrganization.ResourceTypes.All(item => item.Id != organizationResourceType.Id))
            .Select(organizationResourceType =>
                repositoryFactory.OrganizationResourceTypeRepository.Add(mapper.MapToEntity(organizationResourceType, existingOrganization)))
            .ToList();

        repositoryFactory.OrganizationResourceTypeRepository.RemoveRange(itemsToRemove);
        existingOrganization.ResourceTypes = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        return existingOrganization;
    }
}
