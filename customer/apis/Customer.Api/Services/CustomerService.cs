using Customer.Api.Mappers;
using Customer.Shared.Models;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using CustomerOrder = Customer.Shared.Models.CustomerOrder;
using Desk = Customer.Shared.Database.Entities.Desk;
using Location = Customer.Shared.Database.Entities.Location;
using LocationTag = Customer.Shared.Database.Entities.LocationTag;
using Team = Customer.Shared.Database.Entities.Team;

namespace Customer.Api.Services;

public interface ICustomerService
{
    Task<Shared.Models.Customer> GetByIdAsync(string customerId, CancellationToken cancellationToken);
    Task<Shared.Models.Customer> GetMeAsync(bool addCustomerIfNotExist, CancellationToken cancellationToken);

    Task<(bool, Shared.Models.Customer?)> AnyCustomerExistByVerifiableTokenAsync(string verifiableToken,
        CancellationToken cancellationToken);

    Task<(bool, Shared.Models.Customer?)> AnyCustomerExistByEmailAsync(string email,
        CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Customer>>, int )> GetPaginatedCustomersAsync(
        PaginationInputParam paginationInputParam,
        CustomerSearchCriteria searchCriteria,
        ICollection<CustomerOrder> orderByFields,
        CancellationToken cancellationToken);

    Task<Shared.Models.Customer> AddAsync(
        Shared.Models.Customer customer,
        CancellationToken cancellationToken);

    Task<Shared.Models.Customer> AddIdentityAsync(
        Identity identity,
        CancellationToken cancellationToken);

    Task<Shared.Models.Customer> UpdateIdentityAsync(
        Identity identity,
        CancellationToken cancellationToken);
}

public class CustomerService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerOutboxPublisher customerOutboxPublisher,
    IMapper mapper,
    IContext context,
    IRandomHelper randomHelper) : ICustomerService
{
    public async Task<Shared.Models.Customer> GetByIdAsync(string customerId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

        var customer =
            await repositoryFactory.CustomerRepository.GetByIdAsync(
                customerId,
                cancellationToken);
        if (customer is null)
        {
            throw new CustomerNotFound();
        }

        return mapper.MapTo(customer);
    }

    public async Task<Shared.Models.Customer> GetMeAsync(
        bool addCustomerIfNotExist,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.PropertyBag.VerifiableToken);

        var customer =
            await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(
                context.PropertyBag.VerifiableToken!,
                cancellationToken);
        if (customer is not null)
        {
            return mapper.MapTo(customer);
        }

        if (!addCustomerIfNotExist)
        {
            throw new CustomerNotFound();
        }

        return await AddAsync(mapper.MapTo(context.PropertyBag), cancellationToken);
    }

    public async Task<(bool, Shared.Models.Customer?)> AnyCustomerExistByVerifiableTokenAsync(
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(
            verifiableToken,
            cancellationToken);

        return customer is null ? (false, null) : (true, mapper.MapTo(customer));
    }

    public async Task<(bool, Shared.Models.Customer?)> AnyCustomerExistByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.GetByEmailAsync(
            email,
            cancellationToken);

        return customer is null ? (false, null) : (true, mapper.MapTo(customer));
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Customer>>, int)> GetPaginatedCustomersAsync(
        PaginationInputParam paginationInputParam,
        CustomerSearchCriteria searchCriteria,
        ICollection<CustomerOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) =
            await repositoryFactory.CustomerRepository.GetPaginatedCustomersAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                cancellationToken);

        return (paginatedInfo, edges.Select(mapper.MapTo).ToList(), totalCount);
    }

    public async Task<Shared.Models.Customer> AddAsync(
        Shared.Models.Customer customer,
        CancellationToken cancellationToken)
    {
        var existingCustomer =
            await repositoryFactory.CustomerRepository.GetByIdAsync(
                customer.Id,
                cancellationToken);
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

        existingCustomer =
            await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(
                identityToAddOrUpdate.Id,
                cancellationToken);

        if (existingCustomer is null &&
            identityToAddOrUpdate.Email is not null &&
            !string.IsNullOrWhiteSpace(identityToAddOrUpdate.Email))
        {
            existingCustomer =
                await repositoryFactory.CustomerRepository.GetByEmailAsync(
                    identityToAddOrUpdate.Email,
                    cancellationToken);
        }

        var defaultOrganization = customer.DefaultOrganization is null
            ? null
            : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(customer.DefaultOrganization.Id,
                cancellationToken);

        var defaultLocations = new List<Location>();
        foreach (var location in customer.DefaultLocations)
        {
            var organization = location.Organization is null
                ? null
                : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(location.Organization.Id,
                    cancellationToken);

            defaultLocations.Add(await repositoryFactory.LocationRepository.UpsertNakedAsync(
                location.Id,
                organization,
                cancellationToken));
        }

        var defaultTeams = new List<Team>();
        foreach (var team in customer.DefaultTeams)
        {
            var organization = team.Organization is null
                ? null
                : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(team.Organization.Id,
                    cancellationToken);

            defaultTeams.Add(await repositoryFactory.TeamRepository.UpsertNakedAsync(
                team.Id,
                organization,
                cancellationToken));
        }

        var preferredLocationTags = new List<LocationTag>();
        foreach (var locationTag in customer.PreferredLocationTags)
        {
            var location =
                await repositoryFactory.LocationRepository.UpsertNakedAsync(locationTag.Location.Id, null,
                    cancellationToken);

            preferredLocationTags.Add(await repositoryFactory.LocationTagRepository.UpsertNakedAsync(
                locationTag.Id,
                location,
                cancellationToken));
        }

        var preferredDesks = new List<Desk>();
        foreach (var desk in customer.PreferredDesks)
        {
            var location =
                await repositoryFactory.LocationRepository.UpsertNakedAsync(desk.Location.Id, null,
                    cancellationToken);

            preferredDesks.Add(await repositoryFactory.DeskRepository.UpsertNakedAsync(
                desk.Id,
                location,
                cancellationToken));
        }

        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationTagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.DeskRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.CustomerRepository.UnitOfWork,
                cancellationToken);

        if (existingCustomer is null)
        {
            var identities = mapper.MapToEntity(customer.Identities).ToList();
            existingCustomer = mapper.MapToEntity(
                customer,
                identities,
                defaultOrganization,
                defaultLocations,
                defaultTeams,
                preferredLocationTags,
                preferredDesks);

            identities.ForEach(identity => identity.Customer = existingCustomer);
            repositoryFactory.IdentityRepository.AddRange(identities);
            existingCustomer.Identities = identities;
            customer = mapper.MapTo(repositoryFactory.CustomerRepository.Add(existingCustomer));
        }
        else
        {
            existingCustomer.Identities =
                existingCustomer.Identities.Concat([mapper.MapToIdentity(context.PropertyBag)]).ToList();
            customer = mapper.MapTo(repositoryFactory.CustomerRepository.Update(existingCustomer));
        }

        await customerOutboxPublisher.PublishCustomerAsync(
            [customer],
            repositoryFactory.CustomerRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.IdentityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo((await repositoryFactory.CustomerRepository.GetByIdAsync(
            customer.Id,
            cancellationToken))!);
    }

    public async Task<Shared.Models.Customer> AddIdentityAsync(
        Identity identity,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Customer.Id);
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Id);

        var existingCustomer =
            await repositoryFactory.CustomerRepository.GetByIdAsync(identity.Customer.Id, cancellationToken);
        if (existingCustomer is null)
        {
            throw new CustomerNotFound();
        }

        if (existingCustomer.Identities.All(item => item.Id != identity.Id))
        {
            await using var transaction =
                await transactionBuilder.BeginTransactionAsync(repositoryFactory.CustomerRepository.UnitOfWork,
                    cancellationToken);

            var identityToAdd = mapper.MapTo(identity, existingCustomer);
            repositoryFactory.IdentityRepository.Add(identityToAdd);
            existingCustomer.Identities = existingCustomer.Identities.Concat([identityToAdd]).ToList();

            await customerOutboxPublisher.PublishCustomerAsync(
                [mapper.MapTo(repositoryFactory.CustomerRepository.Update(existingCustomer))],
                repositoryFactory.CustomerRepository.UnitOfWork,
                cancellationToken);

            await repositoryFactory.IdentityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        return mapper.MapTo((await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(
            identity.Id,
            cancellationToken))!);
    }

    public async Task<Shared.Models.Customer> UpdateIdentityAsync(
        Identity identity,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Customer.Id);
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Id);

        var existingCustomer =
            await repositoryFactory.CustomerRepository.GetByIdAsync(identity.Customer.Id, cancellationToken);
        if (existingCustomer is null)
        {
            throw new CustomerNotFound();
        }

        var identityToUpdate = existingCustomer.Identities.Single(item => item.Id == identity.Id);

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.CustomerRepository.UnitOfWork,
                cancellationToken);

        var identityToAdd = mapper.MergeTo(identity, identityToUpdate, existingCustomer);
        repositoryFactory.IdentityRepository.Update(identityToAdd);

        await customerOutboxPublisher.PublishCustomerAsync(
            [mapper.MapTo(repositoryFactory.CustomerRepository.Update(existingCustomer))],
            repositoryFactory.CustomerRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.IdentityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo((await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(
            identity.Id,
            cancellationToken))!);
    }
}
