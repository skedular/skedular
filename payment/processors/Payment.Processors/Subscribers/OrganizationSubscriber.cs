using Api.Shared.Clients.Events.Skedular.Organization.V1.Key;
using Enterprise.Shared.Kafka.Consume;
using Payment.Processors.Mappers;
using Payment.Shared.Database.Entities;
using Payment.Shared.Repositories;
using Stripe;
using Customer = Stripe.Customer;
using Event = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event;
using Organization = Payment.Shared.Database.Entities.Organization;
using Type = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Type;

namespace Payment.Processors.Subscribers;

public class OrganizationSubscriber(
    ILogger<OrganizationSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    ICreatable<Customer, CustomerCreateOptions> stripeCustomerCreateService,
    IUpdatable<Customer, CustomerUpdateOptions> stripeCustomerUpdateService)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.OrganizationUpserted:
                {
                    var organization = mapper.MapTo(@event);
                    var existingOrganization =
                        await repositoryFactory.OrganizationRepository.GetByIdAsync(organization.Id, true, true, cancellationToken);
                    if (existingOrganization is not null &&
                        existingOrganization.EventRaisedAt > organization.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Organization event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleOrganizationUpsertedEventAsync(
                        @event,
                        organization,
                        existingOrganization,
                        cancellationToken);
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
        Event @event,
        Shared.Models.Organization organization,
        Organization? existingOrganization,
        CancellationToken cancellationToken)
    {
        if (existingOrganization is null)
        {
            existingOrganization = mapper.MapToEntity(organization);
            var stripeCreatedCustomer = await stripeCustomerCreateService.CreateAsync(
                new CustomerCreateOptions { Name = existingOrganization.Name },
                new RequestOptions { IdempotencyKey = organization.Id },
                cancellationToken);
            existingOrganization.StripeCustomerId = stripeCreatedCustomer.Id;
            existingOrganization = repositoryFactory.OrganizationRepository.Add(existingOrganization);
        }
        else
        {
            existingOrganization = mapper.MergeToEntity(organization, existingOrganization);
            var stripeUpdatedCustomer = await stripeCustomerUpdateService.UpdateAsync(
                existingOrganization.StripeCustomerId,
                new CustomerUpdateOptions { Name = existingOrganization.Name },
                new RequestOptions { IdempotencyKey = @event.Metadata.Id },
                cancellationToken);
            existingOrganization.StripeCustomerId = stripeUpdatedCustomer.Id;
            existingOrganization = repositoryFactory.OrganizationRepository.Update(existingOrganization);
        }

        existingOrganization = await RebuildOrganizationMembersAsync(organization, existingOrganization, cancellationToken);
        _ = await RebuildOrganizationOffering(organization, existingOrganization, cancellationToken);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationOfferingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
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
        var organizationMembers = await repositoryFactory.OrganizationMemberRepository.GetByOrganizationIdAsync(
            existingOrganization.Id,
            cancellationToken);
        var itemsToRemove = organizationMembers
            .Where(organizationMember => organization.OrganizationMembers.All(item => item.Id != organizationMember.Id))
            .ToList();
        var updatedItems = new List<OrganizationMember>();
        foreach (var organizationMember in organizationMembers
                     .Where(organizationMember =>
                         organization.OrganizationMembers.Any(item => item.Id == organizationMember.Id)))
        {
            var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(
                organizationMember.Customer.Id,
                cancellationToken);
            var updatedOrganizationMember = mapper.MergeToEntity(
                organization.OrganizationMembers.First(item => item.Id == organizationMember.Id),
                organizationMember,
                existingOrganization,
                customer);
            updatedOrganizationMember.DeletedAt = null;
            updatedItems.Add(repositoryFactory.OrganizationMemberRepository.Update(updatedOrganizationMember));
        }

        var addedItems = new List<OrganizationMember>();
        foreach (var organizationMember in organization.OrganizationMembers.Where(organizationMember =>
                     organizationMembers.All(item => item.Id != organizationMember.Id)))
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

    private async Task<Organization> RebuildOrganizationOffering(
        Shared.Models.Organization organization,
        Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        var organizationOfferings = await repositoryFactory.OrganizationOfferingRepository.GetByOrganizationIdAsync(
            existingOrganization.Id,
            cancellationToken);
        var itemsToRemove = organizationOfferings
            .Where(organizationOffering =>
                organization.OrganizationOfferings.All(item => item.Id != organizationOffering.Id)).ToList();
        var updatedItems = organizationOfferings
            .Where(organizationOffering =>
                organization.OrganizationOfferings.Any(item => item.Id == organizationOffering.Id)).Select(
                organizationOffering =>
                {
                    var mappedUpdatedOffering = mapper.MergeToEntity(
                        organization.OrganizationOfferings.First(item => item.Id == organizationOffering.Id),
                        organizationOffering, existingOrganization);
                    mappedUpdatedOffering.DeletedAt = null;
                    return repositoryFactory.OrganizationOfferingRepository.Update(mappedUpdatedOffering);
                }).ToList();
        var addedItems = organization.OrganizationOfferings
            .Where(organizationOffering =>
                organizationOfferings.All(item => item.Id != organizationOffering.Id)).Select(
                organizationOffering =>
                    repositoryFactory.OrganizationOfferingRepository.Add(
                        mapper.MapToEntity(organizationOffering, existingOrganization))).ToList();

        repositoryFactory.OrganizationOfferingRepository.RemoveRange(itemsToRemove);
        existingOrganization.OrganizationOfferings = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        return existingOrganization;
    }
}
