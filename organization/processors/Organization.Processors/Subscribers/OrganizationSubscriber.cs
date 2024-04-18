using Api.Shared.Clients.Events.UnityHub.Organization.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Organization.V1.Value;
using Confluent.Kafka;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Kafka.Consume;
using Organization.Processors.Mappers;
using Organization.Shared.Database.Entities;
using Organization.Shared.Repositories;
using Type = Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Type;

namespace Organization.Processors.Subscribers;

public class OrganizationSubscriber(
    ILogger<OrganizationSubscriber> logger,
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IRepositoryFactory repositoryFactory)
    : IEventSubscriber<Key, Event>
{
    public async Task HandleAsync(
        Headers headers,
        Key key,
        Event @event,
        CancellationToken cancellationToken)
    {
        if (@event.Metadata.DomainSource == applicationConfiguration.DomainSource)
        {
            // Event raised previously by this domain, ignoring it.
            return;
        }

        switch (@event.Metadata.Type)
        {
            case Type.OrganizationUpserted:
                {
                    var organization = mapper.MapTo(@event);
                    var existingOrganization =
                        await repositoryFactory.OrganizationRepository.GetByIdAsync(organization.Id, cancellationToken);
                    if (existingOrganization is not null && existingOrganization.ModifiedAt > organization.ModifiedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Organization event. Event timestamp is older that what is already processed.");

                        return;
                    }

                    await HandleOrganizationUpsertedEventAsync(organization, existingOrganization, cancellationToken);
                }
                break;

            case Type.OrganizationDeleted:
                {
                    var organization = mapper.MapTo(@event);
                    var existingOrganization =
                        await repositoryFactory.OrganizationRepository.GetByIdAsync(organization.Id, cancellationToken);
                    if (existingOrganization is not null && existingOrganization.ModifiedAt > organization.ModifiedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Organization event. Event timestamp is older that what is already processed.");

                        return;
                    }

                    if (existingOrganization is null)
                    {
                        return;
                    }

                    await HandleOrganizationDeletedEventAsync(existingOrganization, cancellationToken);
                }
                break;

            case Type.NotificationUpserted:
            case Type.NotificationDeleted:
            case Type.OrganizationOfferingUpdated:
            default:
                return;
        }
    }

    private async Task HandleOrganizationUpsertedEventAsync(
        Shared.Models.Organization organization,
        Shared.Database.Entities.Organization? existingOrganization,
        CancellationToken cancellationToken)
    {
        existingOrganization = existingOrganization is null
            ? repositoryFactory.OrganizationRepository.Add(mapper.MapToEntity(organization))
            : repositoryFactory.OrganizationRepository.Update(mapper.MergeToEntity(organization,
                existingOrganization));

        existingOrganization =
            await RebuildOrganizationMembersAsync(organization, existingOrganization, cancellationToken);
        _ = RebuildOrganizationOffering(organization, existingOrganization);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationOfferingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleOrganizationDeletedEventAsync(
        Shared.Database.Entities.Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        repositoryFactory.OrganizationMemberRepository.RemoveRange(existingOrganization.OrganizationMembers);
        _ = repositoryFactory.OrganizationRepository.Remove(existingOrganization);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Shared.Database.Entities.Organization> RebuildOrganizationMembersAsync(
        Shared.Models.Organization organization,
        Shared.Database.Entities.Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        var itemsToRemove = existingOrganization.OrganizationMembers
            .Where(organizationMember => organization.OrganizationMembers.All(item => item.Id != organizationMember.Id))
            .ToList();
        var updatedItems = new List<OrganizationMember>();
        foreach (var organizationMember in existingOrganization.OrganizationMembers
                     .Where(organizationMember =>
                         organization.OrganizationMembers.Any(item => item.Id == organizationMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(organizationMember.Customer.Id,
                    cancellationToken);
            updatedItems.Add(repositoryFactory.OrganizationMemberRepository.Update(
                mapper.MergeToEntity(
                    organization.OrganizationMembers.Single(item => item.Id == organizationMember.Id),
                    organizationMember,
                    existingOrganization,
                    customer)));
        }

        var addedItems = new List<OrganizationMember>();
        foreach (var organizationMember in organization.OrganizationMembers.Where(organizationMember =>
                     existingOrganization.OrganizationMembers.All(item => item.Id != organizationMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(organizationMember.Customer.Id,
                    cancellationToken);
            addedItems.Add(repositoryFactory.OrganizationMemberRepository.Add(
                mapper.MapToEntity(organizationMember, existingOrganization, customer)));
        }

        repositoryFactory.OrganizationMemberRepository.RemoveRange(itemsToRemove);
        existingOrganization.OrganizationMembers = addedItems.Concat(updatedItems).ToList();

        return existingOrganization;
    }

    private Shared.Database.Entities.Organization RebuildOrganizationOffering(
        Shared.Models.Organization organization,
        Shared.Database.Entities.Organization existingOrganization)
    {
        var itemsToRemove = existingOrganization.OrganizationOfferings
            .Where(organizationOffering =>
                organization.OrganizationOfferings.All(item => item.Id != organizationOffering.Id)).ToList();
        var updatedItems = existingOrganization.OrganizationOfferings
            .Where(organizationOffering =>
                organization.OrganizationOfferings.Any(item => item.Id == organizationOffering.Id)).Select(
                organizationOffering => repositoryFactory.OrganizationOfferingRepository.Update(
                    mapper.MergeToEntity(
                        organization.OrganizationOfferings.Single(item => item.Id == organizationOffering.Id),
                        organizationOffering, existingOrganization))).ToList();
        var addedItems = organization.OrganizationOfferings
            .Where(organizationOffering =>
                existingOrganization.OrganizationOfferings.All(item => item.Id != organizationOffering.Id)).Select(
                organizationOffering =>
                    repositoryFactory.OrganizationOfferingRepository.Add(
                        mapper.MapToEntity(organizationOffering, existingOrganization))).ToList();

        repositoryFactory.OrganizationOfferingRepository.RemoveRange(itemsToRemove);
        existingOrganization.OrganizationOfferings = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        return existingOrganization;
    }
}
