using Api.Shared.Clients.Events.Skedular.Organization.V1.Key;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Random;
using Payment.Processors.Mappers;
using Payment.Shared.Models;
using Payment.Shared.Repositories;
using Stripe;
using Address = Payment.Shared.Models.Address;
using Customer = Stripe.Customer;
using Event = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event;
using Organization = Payment.Shared.Database.Entities.Organization;
using OrganizationMember = Payment.Shared.Database.Entities.OrganizationMember;
using StripeCustomer = Payment.Shared.Database.Entities.StripeCustomer;
using Type = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Type;

namespace Payment.Processors.Subscribers;

public class OrganizationSubscriber(
    ILogger<OrganizationSubscriber> logger,
    IMapper mapper,
    IRandomHelper randomHelper,
    IRepositoryFactory repositoryFactory,
    ICreatable<Customer, CustomerCreateOptions> customerCreateService,
    IUpdatable<Customer, CustomerUpdateOptions> customerUpdateService)
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
                    if (existingOrganization is not null && existingOrganization.EventRaisedAt > organization.EventRaisedAt)
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
                        await repositoryFactory.OrganizationRepository.GetByIdAsync(organization.Id, false, false, cancellationToken);
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
        Event @event,
        Shared.Models.Organization organization,
        Organization? existingOrganization,
        CancellationToken cancellationToken)
    {
        if (existingOrganization is null)
        {
            existingOrganization = mapper.MapToEntity(organization);
            var customer = await customerCreateService.CreateAsync(
                mapper.MapTo(existingOrganization),
                new RequestOptions { IdempotencyKey = organization.Id },
                cancellationToken);

            var stripeCustomerEntity = repositoryFactory.StripeCustomerRepository.Add(new StripeCustomer
            {
                Id = randomHelper.Generate(), StripeCustomerId = customer.Id
            });

            existingOrganization.StripeCustomer = stripeCustomerEntity;
            existingOrganization = repositoryFactory.OrganizationRepository.Add(existingOrganization);
        }
        else
        {
            existingOrganization = mapper.MergeToEntity(organization, existingOrganization);
            if (existingOrganization.StripeCustomer is null)
            {
                var customer = await customerCreateService.CreateAsync(
                    mapper.MapTo(existingOrganization),
                    new RequestOptions { IdempotencyKey = organization.Id },
                    cancellationToken);
                existingOrganization.StripeCustomer = repositoryFactory.StripeCustomerRepository.Add(new StripeCustomer
                {
                    Id = randomHelper.Generate(), StripeCustomerId = customer.Id
                });
            }
            else
            {
                var stripeCustomer = await customerUpdateService.UpdateAsync(
                    existingOrganization.StripeCustomer.StripeCustomerId,
                    mapper.MergeTo(existingOrganization),
                    new RequestOptions { IdempotencyKey = @event.Metadata.Id },
                    cancellationToken);

                existingOrganization.StripeCustomer.StripeCustomerId = stripeCustomer.Id;
                existingOrganization.StripeCustomer = repositoryFactory.StripeCustomerRepository.Update(existingOrganization.StripeCustomer);
            }

            existingOrganization = repositoryFactory.OrganizationRepository.Update(existingOrganization);
        }

        existingOrganization = await RebuildOrganizationMembersAsync(organization, existingOrganization, cancellationToken);
        existingOrganization = await RebuildOrganizationOfferingAsync(organization, existingOrganization, cancellationToken);
        existingOrganization = RebuildOrganizationSsoSettings(organization.OrganizationSsoSettings, existingOrganization);
        _ = RebuildOrganizationPhysicalAddress(organization.PhysicalAddress, existingOrganization);

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
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(organizationMember.Customer.Id, cancellationToken);
            addedItems.Add(
                repositoryFactory.OrganizationMemberRepository.Add(mapper.MapToEntity(organizationMember, existingOrganization, customer)));
        }

        repositoryFactory.OrganizationMemberRepository.RemoveRange(itemsToRemove);
        existingOrganization.OrganizationMembers = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        return existingOrganization;
    }

    private async Task<Organization> RebuildOrganizationOfferingAsync(
        Shared.Models.Organization organization,
        Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        var organizationOfferings = await repositoryFactory.OrganizationOfferingRepository.GetByOrganizationIdAsync(
            existingOrganization.Id,
            cancellationToken);
        var itemsToRemove = organizationOfferings
            .Where(organizationOffering => organization.OrganizationOfferings.All(item => item.Id != organizationOffering.Id)).ToList();
        var updatedItems = organizationOfferings
            .Where(organizationOffering =>
                organization.OrganizationOfferings.Any(item => item.Id == organizationOffering.Id)).Select(organizationOffering =>
            {
                var mappedUpdatedOffering = mapper.MergeToEntity(
                    organization.OrganizationOfferings.First(item => item.Id == organizationOffering.Id),
                    organizationOffering, existingOrganization);
                mappedUpdatedOffering.DeletedAt = null;
                return repositoryFactory.OrganizationOfferingRepository.Update(mappedUpdatedOffering);
            }).ToList();
        var addedItems = organization.OrganizationOfferings
            .Where(organizationOffering =>
                organizationOfferings.All(item => item.Id != organizationOffering.Id)).Select(organizationOffering =>
                repositoryFactory.OrganizationOfferingRepository.Add(
                    mapper.MapToEntity(organizationOffering, existingOrganization))).ToList();

        repositoryFactory.OrganizationOfferingRepository.RemoveRange(itemsToRemove);
        existingOrganization.OrganizationOfferings = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

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

    private Organization RebuildOrganizationPhysicalAddress(Address? address, Organization organization)
    {
        switch (address)
        {
            case null when organization.PhysicalAddress is null:
                // No need to do anything
                break;

            case null when organization.PhysicalAddress is not null:
                repositoryFactory.AddressRepository.Remove(organization.PhysicalAddress);
                break;

            default:
                {
                    if (address is not null && organization.PhysicalAddress is null)
                    {
                        repositoryFactory.AddressRepository.Add(mapper.MapTo(address, organization));
                    }
                    else if (address is not null && organization.PhysicalAddress is not null)
                    {
                        if (address.Id == organization.PhysicalAddress.Id)
                        {
                            repositoryFactory.AddressRepository.Update(mapper.MergeTo(address, organization.PhysicalAddress, organization));
                        }
                        else
                        {
                            repositoryFactory.AddressRepository.Remove(organization.PhysicalAddress);
                            repositoryFactory.AddressRepository.Add(mapper.MapTo(address, organization));
                        }
                    }

                    break;
                }
        }

        return organization;
    }
}
