using Api.Shared.Services;
using Api.Shared.Services.Models;
using Customer.Api.Mappers;
using Customer.Shared.Models;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Customer.Shared.Services.Cache;
using Customer.Shared.Workflows.NewCustomerJoined;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using CustomerOrder = Customer.Shared.Models.CustomerOrder;
using Identity = Customer.Shared.Models.Identity;
using Location = Customer.Shared.Database.Entities.Location;
using OrganizationTag = Customer.Shared.Database.Entities.OrganizationTag;
using Resource = Customer.Shared.Database.Entities.Resource;
using Team = Customer.Shared.Database.Entities.Team;

namespace Customer.Api.Services;

public interface ICustomerService
{
    Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken);
    Task<Shared.Models.Customer> GetByIdAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);
    Task<Shared.Models.Customer> GetMeAsync(bool addCustomerIfNotExist, CancellationToken cancellationToken);
    Task<(bool, Shared.Models.Customer?)> AnyCustomerExistByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<(bool, Shared.Models.Customer?)> AnyCustomerExistByEmailAsync(string email, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Customer>>, int)> GetPaginatedCustomersAsync(
        PaginationInputParam paginationInputParam,
        CustomerSearchCriteria searchCriteria,
        ICollection<CustomerOrder> orderByFields,
        CancellationToken cancellationToken);

    Task<Shared.Models.Customer> AddAsync(Shared.Models.Customer customer, bool sendNewCustomerJoinedEmail, CancellationToken cancellationToken);
    Task<Shared.Models.Customer> AddIdentityAsync(Identity identity, CancellationToken cancellationToken);
    Task<Shared.Models.Customer> UpdateIdentityAsync(Identity identity, CancellationToken cancellationToken);
}

public class CustomerService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerOutboxPublisher customerOutboxPublisher,
    ITemporalOutboxPublisher temporalOutboxPublisher,
    IMapper mapper,
    IContext context,
    IRandomHelper randomHelper,
    ICachedCustomerService cachedCustomerService,
    TimeProvider timeProvider) : ICustomerService
{
    public async Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.GetVerifiableToken());

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(context.GetVerifiableToken(), cancellationToken) ??
                       throw new CustomerNotFound();

        return (mapper.MapTo(customer), customer);
    }

    public async Task<Shared.Models.Customer> GetByIdAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customer = await cachedCustomerService.GetByIdAsync(id, cancellationToken) ?? throw new CustomerNotFound();
        if (!ignoreAuthorizationCheck)
        {
            var callingCustomer = await cachedCustomerService.GetAsync(cancellationToken);
            if (callingCustomer.Id != id)
            {
                var askingCustomerOrganizations = await repositoryFactory.OrganizationRepository.GetByCustomerIdAsync(customer.Id, cancellationToken);
                var callingCustomerOrganizations =
                    await repositoryFactory.OrganizationRepository.GetByCustomerIdAsync(callingCustomer.Id, cancellationToken);

                var mutualOrganizations = askingCustomerOrganizations
                    .Where(item => callingCustomerOrganizations.Select(organization => organization.Id).Contains(item.Id))
                    .ToList();
                if (mutualOrganizations.Count == 0)
                {
                    throw new UnauthorizedAccessException();
                }

                if (mutualOrganizations.All(item => item.MemberVisibilityPolicy != OrganizationMemberVisibilityPolicyConstants.FullAccess))
                {
                    var result = mapper.MapTo(customer);
                    result = result.Redact(OrganizationMemberVisibilityPolicy.LimitedAccess);
                    foreach (var identity in result.Identities)
                    {
                        identity.Email = identity.Email.FullRedact(OrganizationMemberVisibilityPolicy.LimitedAccess);
                    }

                    return result;
                }
            }
        }

        return mapper.MapTo(customer);
    }

    public async Task<Shared.Models.Customer> GetMeAsync(bool addCustomerIfNotExist, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.GetVerifiableToken());

        var customer = await cachedCustomerService.GetNullableAsync(cancellationToken);
        if (customer is not null)
        {
            return mapper.MapTo(customer);
        }

        if (!addCustomerIfNotExist)
        {
            throw new CustomerNotFound();
        }

        return await AddAsync(mapper.MapTo(context), true, cancellationToken);
    }

    public async Task<(bool, Shared.Models.Customer?)> AnyCustomerExistByVerifiableTokenAsync(
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetByVerifiableTokenAsync(verifiableToken, cancellationToken);
        return customer is null ? (false, null) : (true, mapper.MapTo(customer));
    }

    public async Task<(bool, Shared.Models.Customer?)> AnyCustomerExistByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.GetByEmailAsync(email, cancellationToken);
        return customer is null ? (false, null) : (true, mapper.MapTo(customer));
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Customer>>, int)> GetPaginatedCustomersAsync(
        PaginationInputParam paginationInputParam,
        CustomerSearchCriteria searchCriteria,
        ICollection<CustomerOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await repositoryFactory.CustomerRepository.GetPaginatedCustomersAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        await cachedCustomerService.UpdateAsync(edges.Select(item => item.Node).ToList(), cancellationToken);

        return (paginatedInfo, edges.Select(mapper.MapTo).ToList(), totalCount);
    }

    public async Task<Shared.Models.Customer> AddAsync(
        Shared.Models.Customer customer,
        bool sendNewCustomerJoinedEmail,
        CancellationToken cancellationToken)
    {
        var existingCustomer = await repositoryFactory.CustomerRepository.GetByIdAsync(customer.Id, cancellationToken);
        if (existingCustomer is not null)
        {
            return mapper.MapTo(existingCustomer);
        }

        if (string.IsNullOrWhiteSpace(customer.Id))
        {
            customer.Id = randomHelper.Generate();
        }

        var identityToAddOrUpdate = customer.Identities.FirstOrDefault();
        if (identityToAddOrUpdate is null)
        {
            throw new InvalidOperationException("no identity provided");
        }

        if (string.IsNullOrWhiteSpace(identityToAddOrUpdate.Id))
        {
            throw new InvalidOperationException("identity.Id is empty");
        }

        existingCustomer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(identityToAddOrUpdate.Id, cancellationToken);
        if (existingCustomer is null &&
            identityToAddOrUpdate.Email is not null &&
            !string.IsNullOrWhiteSpace(identityToAddOrUpdate.Email))
        {
            existingCustomer = await repositoryFactory.CustomerRepository.GetByEmailAsync(identityToAddOrUpdate.Email, cancellationToken);
        }

        var defaultOrganization = customer.DefaultOrganization is null
            ? null
            : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(customer.DefaultOrganization.Id, cancellationToken);

        var preferredLocations = new List<Location>();
        foreach (var location in customer.PreferredLocations)
        {
            preferredLocations.Add(await repositoryFactory.LocationRepository.UpsertNakedAsync(location.Id, null, cancellationToken));
        }

        var preferredTeams = new List<Team>();
        foreach (var team in customer.PreferredTeams)
        {
            preferredTeams.Add(await repositoryFactory.TeamRepository.UpsertNakedAsync(team.Id, null, cancellationToken));
        }

        var preferredResources = new List<Resource>();
        foreach (var resource in customer.PreferredResources)
        {
            var location = resource.Location is null
                ? null
                : await repositoryFactory.LocationRepository.UpsertNakedAsync(resource.Location.Id, null, cancellationToken);
            preferredResources.Add(await repositoryFactory.ResourceRepository.UpsertNakedAsync(resource.Id, location, cancellationToken));
        }

        var preferredOrganizationTags = new List<OrganizationTag>();
        foreach (var organizationTag in customer.PreferredOrganizationTags)
        {
            var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationTag.Organization.Id, cancellationToken);
            preferredOrganizationTags.Add(
                await repositoryFactory.OrganizationTagRepository.UpsertNakedAsync(
                    organizationTag.Id,
                    organization,
                    cancellationToken));
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (existingCustomer is null)
        {
            var identities = mapper.MapToEntity(customer.Identities).ToList();
            existingCustomer = mapper.MapToEntity(
                customer,
                identities,
                defaultOrganization,
                preferredLocations,
                preferredTeams,
                preferredResources,
                preferredOrganizationTags);

            identities.ForEach(identity => identity.Customer = existingCustomer);
            repositoryFactory.IdentityRepository.AddRange(identities);
            existingCustomer.Identities = identities;
            existingCustomer = repositoryFactory.CustomerRepository.Add(existingCustomer);
            customer = mapper.MapTo(existingCustomer);
        }
        else
        {
            var identity = mapper.MapToIdentity(context);
            identity.CreatedAt = timeProvider.GetUtcNow();
            existingCustomer.Identities = existingCustomer.Identities.Concat([identity]).ToList();
            existingCustomer = repositoryFactory.CustomerRepository.Update(existingCustomer);
            customer = mapper.MapTo(existingCustomer);
        }

        customerOutboxPublisher.PublishCustomers([customer], repositoryFactory.UnitOfWork);

        if (sendNewCustomerJoinedEmail)
        {
            temporalOutboxPublisher.StartWorkflowNewCustomerJoined(new NewCustomerJoinedInput(customer.Id), repositoryFactory.UnitOfWork);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedCustomerService.UpdateAsync([existingCustomer], cancellationToken);

        return mapper.MapTo((await repositoryFactory.CustomerRepository.GetByIdAsync(customer.Id, cancellationToken))!);
    }

    public async Task<Shared.Models.Customer> AddIdentityAsync(Identity identity, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Customer.Id);
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Id);

        var existingCustomer = await repositoryFactory.CustomerRepository.GetByIdAsync(identity.Customer.Id, cancellationToken) ??
                               throw new CustomerNotFound();
        if (existingCustomer.Identities.All(item => item.Id != identity.Id))
        {
            await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

            var identityToAdd = mapper.MapTo(identity, existingCustomer);
            repositoryFactory.IdentityRepository.Add(identityToAdd);
            existingCustomer.Identities = existingCustomer.Identities.Concat([identityToAdd]).ToList();
            existingCustomer = repositoryFactory.CustomerRepository.Update(existingCustomer);

            customerOutboxPublisher.PublishCustomers([mapper.MapTo(existingCustomer)], repositoryFactory.UnitOfWork);

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await cachedCustomerService.UpdateAsync([existingCustomer], cancellationToken);
        }

        return mapper.MapTo((await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(identity.Id, cancellationToken))!);
    }

    public async Task<Shared.Models.Customer> UpdateIdentityAsync(Identity identity, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Customer.Id);
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Id);

        var existingCustomer = await repositoryFactory.CustomerRepository.GetByIdAsync(identity.Customer.Id, cancellationToken) ??
                               throw new CustomerNotFound();
        var matchingIdentityToUpdate = existingCustomer.Identities.First(item => item.Id == identity.Id);
        var identityChanged = identity.Email != matchingIdentityToUpdate.Email || identity.EmailVerified != matchingIdentityToUpdate.EmailVerified;

        if (identityChanged)
        {
            await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

            var identityToUpdate = mapper.MergeTo(identity, matchingIdentityToUpdate, existingCustomer);
            repositoryFactory.IdentityRepository.Update(identityToUpdate);
            existingCustomer = repositoryFactory.CustomerRepository.Update(existingCustomer);

            customerOutboxPublisher.PublishCustomers([mapper.MapTo(existingCustomer)], repositoryFactory.UnitOfWork);

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await cachedCustomerService.UpdateAsync([existingCustomer], cancellationToken);
        }

        return mapper.MapTo((await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(identity.Id, cancellationToken))!);
    }
}
