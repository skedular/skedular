using Api.Shared.Services;
using Api.Shared.Services.Models;
using Customer.Api.Models;
using Customer.Shared.Mappers;
using Customer.Shared.Models;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Customer.Shared.Services;
using Customer.Shared.Services.Cache;
using Customer.Shared.Workflows;
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

namespace Customer.Api.Services;

public interface ICustomerService
{
    Task<Shared.Models.Customer> GetMeAsync(bool addCustomerIfNotExist, CancellationToken cancellationToken);
    Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken);
    Task<Shared.Models.Customer> GetByIdAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);
    Task<(bool, Shared.Models.Customer?)> AnyCustomerExistByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<(bool, Shared.Models.Customer?)> AnyCustomerExistByEmailAsync(string email, CancellationToken cancellationToken);

    Task<(PaginatedInfo, IReadOnlyList<Edge<Shared.Models.Customer>>, int)> GetPaginatedCustomersAsync(
        PaginationInputParam paginationInputParam,
        CustomerSearchCriteria searchCriteria,
        IEnumerable<CustomerOrder> orderByFields,
        CancellationToken cancellationToken);

    Task<Shared.Models.Customer> AddAsync(Shared.Models.Customer customer, bool sendNewCustomerJoinedEmail, CancellationToken cancellationToken);
    Task<Shared.Models.Customer> AddIdentityAsync(Identity identity, CancellationToken cancellationToken);
    Task<Shared.Models.Customer> UpdateIdentityAsync(CustomerIdentityPatchRequest request, CancellationToken cancellationToken);
}

public class CustomerService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerOutboxPublisher customerOutboxPublisher,
    ITemporalOutboxService temporalOutboxService,
    IEntityMapper entityMapper,
    IContext context,
    IRandomHelper randomHelper,
    ICachedCustomerService cachedCustomerService,
    TimeProvider timeProvider,
    ILogger<CustomerService> logger) : ICustomerService
{
    public async Task<Shared.Models.Customer> GetMeAsync(bool addCustomerIfNotExist, CancellationToken cancellationToken)
    {
        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await cachedCustomerService.GetNullableAsync(cancellationToken);
        if (customer is not null)
        {
            return entityMapper.MapTo(customer);
        }

        if (!addCustomerIfNotExist)
        {
            throw new CustomerNotFound();
        }

        return await AddAsync(entityMapper.MapTo(context), true, cancellationToken);
    }

    public async Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken)
    {
        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, cancellationToken) ??
                       throw new CustomerNotFound();

        return (entityMapper.MapTo(customer), customer);
    }

    public async Task<Shared.Models.Customer> GetByIdAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customer = await cachedCustomerService.GetByIdAsync(id, cancellationToken) ?? throw new CustomerNotFound();
        var mappedCustomer = entityMapper.MapTo(customer);

        if (ignoreAuthorizationCheck)
        {
            return mappedCustomer;
        }

        var me = await cachedCustomerService.GetAsync(cancellationToken) ?? throw new CustomerNotFound();
        if (me.Id == customer.Id)
        {
            return mappedCustomer;
        }

        if (mappedCustomer.PersonalInformationVisibility == PersonalInformationVisibility.Redacted)
        {
            return mappedCustomer.Redact(mappedCustomer.PersonalInformationVisibility);
        }

        mappedCustomer.Identities = [];
        mappedCustomer.BillingDetails = null;
        mappedCustomer.StripePaymentMethods = [];
        mappedCustomer.StripeCustomer = null;

        return mappedCustomer;
    }

    public async Task<(bool, Shared.Models.Customer?)> AnyCustomerExistByVerifiableTokenAsync(
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetByVerifiableTokenAsync(verifiableToken, cancellationToken);
        return customer is null ? (false, null) : (true, entityMapper.MapTo(customer));
    }

    public async Task<(bool, Shared.Models.Customer?)> AnyCustomerExistByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.GetByEmailUntrackedAsync(email, cancellationToken);
        return customer is null ? (false, null) : (true, entityMapper.MapTo(customer));
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<Shared.Models.Customer>>, int)> GetPaginatedCustomersAsync(
        PaginationInputParam paginationInputParam,
        CustomerSearchCriteria searchCriteria,
        IEnumerable<CustomerOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await repositoryFactory.CustomerRepository.GetPaginatedCustomersUntrackedAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        return (paginatedInfo, edges.Select(entityMapper.MapTo).ToList(), totalCount);
    }

    public async Task<Shared.Models.Customer> AddAsync(
        Shared.Models.Customer customer,
        bool sendNewCustomerJoinedEmail,
        CancellationToken cancellationToken)
    {
        var existingCustomer = await repositoryFactory.CustomerRepository.GetByIdAsync(customer.Id, cancellationToken);
        if (existingCustomer is not null)
        {
            return entityMapper.MapTo(existingCustomer);
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
        if (existingCustomer is null && identityToAddOrUpdate.Email is not null && !string.IsNullOrWhiteSpace(identityToAddOrUpdate.Email))
        {
            existingCustomer = await repositoryFactory.CustomerRepository.GetByEmailAsync(identityToAddOrUpdate.Email, cancellationToken);
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var defaultOrganization = customer.DefaultOrganization is null
            ? null
            : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(customer.DefaultOrganization.Id, cancellationToken);

        var preferredLocations = new List<Location>();
        foreach (var location in customer.PreferredLocations)
        {
            preferredLocations.Add(await repositoryFactory.LocationRepository.UpsertNakedAsync(location.Id, null, cancellationToken));
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

        var favouriteLocations = new List<Location>();
        foreach (var location in customer.FavouriteLocations)
        {
            favouriteLocations.Add(await repositoryFactory.LocationRepository.UpsertNakedAsync(location.Id, null, cancellationToken));
        }

        if (existingCustomer is null)
        {
            var identities = entityMapper.MapToEntity(customer.Identities).ToList();
            existingCustomer = entityMapper.MapToEntity(
                customer,
                identities,
                defaultOrganization,
                preferredLocations,
                preferredResources,
                preferredOrganizationTags,
                favouriteLocations);

            repositoryFactory.IdentityRepository.AddRange(identities);
            existingCustomer.Identities = identities;
            existingCustomer = repositoryFactory.CustomerRepository.Add(existingCustomer);
            customer = entityMapper.MapTo(existingCustomer);
        }
        else
        {
            var identity = entityMapper.MapToIdentity(context);
            identity.CreatedAt = timeProvider.GetUtcNow();
            existingCustomer.Identities = existingCustomer.Identities.Append(identity).ToList();
            existingCustomer = repositoryFactory.CustomerRepository.Update(existingCustomer);
            customer = entityMapper.MapTo(existingCustomer);
        }

        customerOutboxPublisher.PublishCustomers([customer], repositoryFactory.UnitOfWork);

        if (sendNewCustomerJoinedEmail)
        {
            temporalOutboxService.StartWorkflowNewCustomerJoined(new NewCustomerJoinedInput(customer.Id), repositoryFactory.UnitOfWork);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        existingCustomer = (await repositoryFactory.CustomerRepository.GetByIdUntrackedAsync(customer.Id, cancellationToken))!;

        await cachedCustomerService.UpdateAsync([existingCustomer], cancellationToken);

        return entityMapper.MapTo(existingCustomer);
    }

    public async Task<Shared.Models.Customer> AddIdentityAsync(Identity identity, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Customer.Id);
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Id);

        var existingCustomer = await repositoryFactory.CustomerRepository.GetByIdAsync(identity.Customer.Id, cancellationToken) ??
                               throw new CustomerNotFound();
        Shared.Database.Entities.Customer? existingCustomerToCache = null;

        if (existingCustomer.Identities.All(item => item.Id != identity.Id))
        {
            await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

            var identityToAdd = entityMapper.MapTo(identity, existingCustomer);
            repositoryFactory.IdentityRepository.Add(identityToAdd);
            existingCustomer.Identities = existingCustomer.Identities.Append(identityToAdd).ToList();
            existingCustomer = repositoryFactory.CustomerRepository.Update(existingCustomer);

            customerOutboxPublisher.PublishCustomers([entityMapper.MapTo(existingCustomer)], repositoryFactory.UnitOfWork);

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            existingCustomerToCache =
                (await repositoryFactory.CustomerRepository.GetByVerifiableTokenUntrackedAsync(identity.Id, cancellationToken))!;
            await cachedCustomerService.UpdateAsync([existingCustomerToCache], cancellationToken);
        }

        return entityMapper.MapTo(existingCustomerToCache ??
                                  (await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(identity.Id, cancellationToken))!);
    }

    public async Task<Shared.Models.Customer> UpdateIdentityAsync(CustomerIdentityPatchRequest request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Identity.Customer.Id);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Identity.Id);

        var editUnits = string.Join(",", request.FieldsToUpdate);
        logger.LogInformation(
            "Customer identity patch update started. CustomerId: {CustomerId}, IdentityId: {IdentityId}, EditUnits: {EditUnits}",
            request.Identity.Customer.Id,
            request.Identity.Id,
            editUnits);

        try
        {
            var existingCustomer = await repositoryFactory.CustomerRepository.GetByIdAsync(request.Identity.Customer.Id, cancellationToken) ??
                                   throw new CustomerNotFound();
            var identity = existingCustomer.Identities.First(item => item.Id == request.Identity.Id);
            var patch = new Identity
            {
                Id = identity.Id,
                Customer = new Shared.Models.Customer { Id = existingCustomer.Id },
                Email = identity.Email,
                EmailVerified = identity.EmailVerified
            };

            foreach (var field in request.FieldsToUpdate)
            {
                switch (field)
                {
                    case CustomerIdentityPatchField.Email:
                        patch.Email = request.Identity.Email;
                        break;
                    case CustomerIdentityPatchField.EmailVerified:
                        patch.EmailVerified = request.Identity.EmailVerified;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(request.FieldsToUpdate), field, null);
                }
            }

            var updatedCustomer = await UpdateIdentityAsync(patch, cancellationToken);
            logger.LogInformation(
                "Customer identity patch update completed. CustomerId: {CustomerId}, IdentityId: {IdentityId}, EditUnits: {EditUnits}",
                updatedCustomer.Id,
                request.Identity.Id,
                editUnits);
            return updatedCustomer;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Customer identity patch update failed. CustomerId: {CustomerId}, IdentityId: {IdentityId}, EditUnits: {EditUnits}",
                request.Identity.Customer.Id,
                request.Identity.Id,
                editUnits);
            throw;
        }
    }

    private async Task<Shared.Models.Customer> UpdateIdentityAsync(Identity identity, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Customer.Id);
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Id);

        var existingCustomer = await repositoryFactory.CustomerRepository.GetByIdAsync(identity.Customer.Id, cancellationToken) ??
                               throw new CustomerNotFound();
        var matchingIdentityToUpdate = existingCustomer.Identities.First(item => item.Id == identity.Id);
        var identityChanged = identity.Email != matchingIdentityToUpdate.Email || identity.EmailVerified != matchingIdentityToUpdate.EmailVerified;
        Shared.Database.Entities.Customer? existingCustomerToCache = null;

        if (identityChanged)
        {
            await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

            var identityToUpdate = entityMapper.MergeTo(identity, matchingIdentityToUpdate, existingCustomer);
            repositoryFactory.IdentityRepository.Update(identityToUpdate);
            existingCustomer = repositoryFactory.CustomerRepository.Update(existingCustomer);

            customerOutboxPublisher.PublishCustomers([entityMapper.MapTo(existingCustomer)], repositoryFactory.UnitOfWork);

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            existingCustomerToCache =
                (await repositoryFactory.CustomerRepository.GetByVerifiableTokenUntrackedAsync(identity.Id, cancellationToken))!;
            await cachedCustomerService.UpdateAsync([existingCustomerToCache], cancellationToken);
        }

        return entityMapper.MapTo(existingCustomerToCache ??
                                  (await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(identity.Id, cancellationToken))!);
    }
}
