using Api.Shared.Clients.Events.Skedular.Organization.V1.Key;
using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Services;
using Customer.Processors.Mappers;
using Customer.Shared.Models;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Customer.Shared.Services.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using Microsoft.EntityFrameworkCore;
using Organization = Customer.Shared.Database.Entities.Organization;
using OrganizationMember = Customer.Shared.Database.Entities.OrganizationMember;
using Type = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Type;

namespace Customer.Processors.Subscribers;

public class OrganizationSubscriber(
    ILogger<OrganizationSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    ICustomerPublisher customerPublisher,
    ICachedOrganizationService cachedOrganizationService,
    ICachedCustomerService cachedCustomerService)
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
                    var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                        organization.Id,
                        null,
                        true,
                        true,
                        cancellationToken);
                    if (existingOrganization is not null &&
                        existingOrganization.EventRaisedAt > organization.EventRaisedAt)
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

            case Type.OrganizationOfferingUpdated:
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleOrganizationUpsertedEventAsync(
        Shared.Models.Organization organization,
        Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        existingOrganization = repositoryFactory.OrganizationRepository.Update(mapper.MergeToEntity(organization, existingOrganization));
        existingOrganization = RebuildOrganizationTags(organization, existingOrganization);
        existingOrganization = await RebuildOrganizationMembersAsync(organization, existingOrganization, cancellationToken);
        _ = RebuildOrganizationSsoSettings(organization.OrganizationSsoSettings, existingOrganization);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await cachedOrganizationService.UpdateByIdOrUniqueAlphanumericNameAsync(
            existingOrganization.Id,
            existingOrganization.UniqueAlphanumericName,
            cancellationToken);
    }

    private async Task HandleOrganizationDeletedEventAsync(Organization existingOrganization, CancellationToken cancellationToken)
    {
        var customers = await UpdateOrganizationMembersDefaultOrganizationAsync(
            existingOrganization.Id, existingOrganization.OrganizationMembers,
            cancellationToken);
        customers = customers.Concat(await UpdateCustomerDefaultOrganizationAsync(existingOrganization, cancellationToken)).ToList();
        repositoryFactory.OrganizationMemberRepository.RemoveRange(existingOrganization.OrganizationMembers);
        existingOrganization.UniqueAlphanumericName = null;
        _ = repositoryFactory.OrganizationRepository.Remove(existingOrganization);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await cachedOrganizationService.RemoveByIdOrUniqueAlphanumericNameAsync(
            existingOrganization.Id,
            existingOrganization.UniqueAlphanumericName,
            cancellationToken);

        foreach (var customer in customers)
        {
            await cachedCustomerService.UpdateByIdAsync(customer.Id, cancellationToken);
            foreach (var item in customer.Identities)
            {
                await cachedCustomerService.UpdateByVerifiableTokenAsync(item.Id, cancellationToken);
            }
        }
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
        foreach (var organizationMember in organizationMembers.Where(organizationMember =>
                     organization.OrganizationMembers.Any(item => item.Id == organizationMember.Id)))
        {
            var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(organizationMember.Customer.Id, cancellationToken) ??
                           throw new CustomerNotFound();
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
            var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(organizationMember.Customer.Id, cancellationToken) ??
                           throw new CustomerNotFound();
            addedItems.Add(
                repositoryFactory.OrganizationMemberRepository.Add(mapper.MapToEntity(organizationMember, existingOrganization, customer)));
        }

        await UpdateOrganizationMembersDefaultOrganizationAsync(organization.Id, itemsToRemove, cancellationToken);

        repositoryFactory.OrganizationMemberRepository.RemoveRange(itemsToRemove);
        existingOrganization.OrganizationMembers = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        return existingOrganization;
    }

    private async Task<ICollection<Shared.Database.Entities.Customer>> UpdateOrganizationMembersDefaultOrganizationAsync(
        string organizationId,
        IEnumerable<OrganizationMember> organizationMembersToRemove,
        CancellationToken cancellationToken)
    {
        var organizationMemberIds = organizationMembersToRemove.Select(organizationMember => organizationMember.Id).ToList();
        var customers = new List<Shared.Database.Entities.Customer>();

        foreach (var organizationMemberId in organizationMemberIds)
        {
            var member = await repositoryFactory.OrganizationMemberRepository.Query(
                    new Specification<OrganizationMember> { Criteria = query => query.Id == organizationMemberId }
                        .AddInclude(query => query.Customer))
                .FirstAsync(cancellationToken);

            var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(member.Customer.Id, cancellationToken) ??
                           throw new CustomerNotFound();
            var existingOrganizationId = customer.DefaultOrganization?.Id;

            if (customer.DefaultOrganization is not null && customer.DefaultOrganization.Id == organizationId)
            {
                customer.DefaultOrganization = null;
            }

            var newOrganizationId = customer.DefaultOrganization?.Id;

            var existingLocationIds = customer.PreferredLocations.Select(item => item.Id).Distinct().ToList();
            customer.PreferredLocations = customer.PreferredLocations
                .Where(location => location.Organization is not null && location.Organization.Id != organizationId)
                .ToList();
            var newLocationIds = customer.PreferredLocations.Select(item => item.Id).Distinct().ToList();

            var existingResourceIds = customer.PreferredResources.Select(item => item.Id).Distinct().ToList();
            customer.PreferredResources = customer.PreferredResources
                .Where(resource => resource.Location?.Organization is null || resource.Location.Organization.Id != organizationId)
                .ToList();
            var newResourceIds = customer.PreferredResources.Select(item => item.Id).Distinct().ToList();

            var existingTeamIds = customer.PreferredTeams.Select(item => item.Id).Distinct().ToList();
            customer.PreferredTeams = customer.PreferredTeams
                .Where(team => team.Organization is not null && team.Organization.Id != organizationId)
                .ToList();
            var newTeamIds = customer.PreferredTeams.Select(item => item.Id).Distinct().ToList();

            customer = repositoryFactory.CustomerRepository.Update(customer);
            customers.Add(customer);

            if (existingOrganizationId != newOrganizationId ||
                newLocationIds.Count != existingLocationIds.Count ||
                newLocationIds.Except(existingLocationIds).Any() ||
                newResourceIds.Count != existingResourceIds.Count ||
                newResourceIds.Except(existingResourceIds).Any() ||
                newTeamIds.Count != existingTeamIds.Count ||
                newTeamIds.Except(existingTeamIds).Any())
            {
                await customerPublisher.PublishCustomersAsync([mapper.MapTo(customer)!], cancellationToken);
            }
        }

        return customers;
    }

    private async Task<ICollection<Shared.Database.Entities.Customer>> UpdateCustomerDefaultOrganizationAsync(
        Organization organization,
        CancellationToken cancellationToken)
    {
        var customerIds = organization.DefaultedByCustomers.Select(customer => customer.Id).ToList();
        var customers = new List<Shared.Database.Entities.Customer>();

        foreach (var customerId in customerIds)
        {
            var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(customerId, cancellationToken) ?? throw new CustomerNotFound();
            customer.DefaultOrganization = null;
            _ = repositoryFactory.CustomerRepository.Update(customer);
            customers.Add(customer);
        }

        return customers;
    }

    private Organization RebuildOrganizationTags(Shared.Models.Organization organization, Organization existingOrganization)
    {
        var itemsToRemove = existingOrganization.Tags.Where(tag => organization.Tags.All(item => item.Id != tag.Id)).ToList();
        var updatedItems = existingOrganization.Tags
            .Where(tag => organization.Tags.Any(item => item.Id == tag.Id))
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
            .Where(tag => existingOrganization.Tags.All(item => item.Id != tag.Id))
            .Select(organizationTag => repositoryFactory.OrganizationTagRepository.Add(mapper.MapToEntity(organizationTag, existingOrganization)))
            .ToList();

        repositoryFactory.OrganizationTagRepository.RemoveRange(itemsToRemove);
        existingOrganization.Tags = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        return existingOrganization;
    }

    private Organization RebuildOrganizationSsoSettings(OrganizationSsoSetting? ssoSettings, Organization organization)
    {
        switch (ssoSettings)
        {
            case null when organization.OrganizationSsoSettings is null:
                // No need to do anything
                break;

            case null when organization.OrganizationSsoSettings is not null:
                repositoryFactory.OrganizationSsoSettingRepository.Remove(organization.OrganizationSsoSettings);
                break;

            default:
                {
                    if (ssoSettings is not null && organization.OrganizationSsoSettings is null)
                    {
                        repositoryFactory.OrganizationSsoSettingRepository.Add(mapper.MapTo(ssoSettings, organization));
                    }
                    else if (ssoSettings is not null && organization.OrganizationSsoSettings is not null)
                    {
                        if (ssoSettings.Id == organization.OrganizationSsoSettings.Id)
                        {
                            repositoryFactory.OrganizationSsoSettingRepository.Update(
                                mapper.MergeTo(ssoSettings, organization.OrganizationSsoSettings, organization));
                        }
                        else
                        {
                            repositoryFactory.OrganizationSsoSettingRepository.Remove(organization.OrganizationSsoSettings);
                            repositoryFactory.OrganizationSsoSettingRepository.Add(mapper.MapTo(ssoSettings, organization));
                        }
                    }

                    break;
                }
        }

        return organization;
    }
}
