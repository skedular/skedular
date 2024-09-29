using Api.Shared.Clients.Events.UnityHub.Organization.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Organization.V1.Value;
using Confluent.Kafka;
using Customer.Processors.Mappers;
using Customer.Shared.Database.Entities;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using Microsoft.EntityFrameworkCore;
using Organization = Customer.Shared.Database.Entities.Organization;
using Type = Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Type;

namespace Customer.Processors.Subscribers;

public class OrganizationSubscriber(
    ILogger<OrganizationSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    ICustomerPublisher customerPublisher)
    : IEventSubscriber<Key, Event>
{
    public async Task HandleAsync(
        Headers headers,
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
                    if (existingOrganization is not null &&
                        existingOrganization.EventRaisedAt > organization.EventRaisedAt)
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

            case Type.InvitationToJoinOrganizationUpserted:
            case Type.InvitationToJoinOrganizationDeleted:
            case Type.OrganizationOfferingUpdated:
            default:
                return;
        }
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

        _ = await RebuildOrganizationMembersAsync(organization, existingOrganization, cancellationToken);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleOrganizationDeletedEventAsync(
        Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        await UpdateOrganizationMembersDefaultOrganizationAsync(
            existingOrganization.Id,
            existingOrganization.OrganizationMembers,
            cancellationToken);
        await UpdateCustomerDefaultOrganizationAsync(existingOrganization, cancellationToken);
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
        foreach (var organizationMember in organization.OrganizationMembers
                     .Where(organizationMember =>
                         existingOrganization.OrganizationMembers.All(item => item.Id != organizationMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(organizationMember.Customer.Id,
                    cancellationToken);
            addedItems.Add(repositoryFactory.OrganizationMemberRepository.Add(mapper.MapToEntity(
                organizationMember,
                existingOrganization,
                customer)));
        }

        await UpdateOrganizationMembersDefaultOrganizationAsync(organization.Id, itemsToRemove,
            cancellationToken);

        repositoryFactory.OrganizationMemberRepository.RemoveRange(itemsToRemove);
        existingOrganization.OrganizationMembers = addedItems.Concat(updatedItems).ToList();

        return existingOrganization;
    }

    private async Task UpdateOrganizationMembersDefaultOrganizationAsync(
        string organizationId,
        IEnumerable<OrganizationMember> organizationMembersToRemove,
        CancellationToken cancellationToken)
    {
        var organizationMemberIds =
            organizationMembersToRemove.Select(organizationMember => organizationMember.Id).ToList();
        foreach (var organizationMemberId in organizationMemberIds)
        {
            var member = await repositoryFactory.OrganizationMemberRepository
                .Query(new Specification<OrganizationMember> { Criteria = query => query.Id == organizationMemberId }
                    .AddInclude(query => query.Customer))
                .FirstAsync(cancellationToken);

            var customer =
                await repositoryFactory.CustomerRepository.GetByIdAsync(member.Customer.Id, cancellationToken);
            ArgumentNullException.ThrowIfNull(customer);

            if (customer.DefaultOrganization is not null && customer.DefaultOrganization.Id == organizationId)
            {
                customer.DefaultOrganization = null;
            }

            customer.DefaultLocations = customer.DefaultLocations
                .Where(location => location.Organization is null || location.Organization.Id != organizationId)
                .ToList();
            customer.PreferredLocationTags = customer.PreferredLocationTags
                .Where(locationTag => locationTag.Location.Organization is null ||
                                      locationTag.Location.Organization.Id != organizationId).ToList();
            customer.PreferredDesks = customer.PreferredDesks
                .Where(desk => desk.Location.Organization is null || desk.Location.Organization.Id != organizationId)
                .ToList();
            customer.DefaultTeams = customer.DefaultTeams
                .Where(team => team.Organization is null || team.Organization.Id != organizationId).ToList();

            customer = repositoryFactory.CustomerRepository.Update(customer);
            await customerPublisher.PublishCustomerAsync([mapper.MapTo(customer)!], cancellationToken);
        }
    }

    private async Task UpdateCustomerDefaultOrganizationAsync(
        Organization organization,
        CancellationToken cancellationToken)
    {
        var customerIds = organization.DefaultedByCustomers.Select(customer => customer.Id).ToList();
        foreach (var customerId in customerIds)
        {
            var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
            ArgumentNullException.ThrowIfNull(customer);

            customer.DefaultOrganization = null;
            _ = repositoryFactory.CustomerRepository.Update(customer);
        }
    }
}
