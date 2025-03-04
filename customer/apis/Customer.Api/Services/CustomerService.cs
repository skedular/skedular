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
using Room = Customer.Shared.Database.Entities.Room;
using Location = Customer.Shared.Database.Entities.Location;
using OrganizationTag = Customer.Shared.Database.Entities.OrganizationTag;
using Team = Customer.Shared.Database.Entities.Team;

namespace Customer.Api.Services;

public interface ICustomerService
{
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
    IMapper mapper,
    IContext context,
    IRandomHelper randomHelper,
    INotificationOutboxPublisher notificationOutboxPublisher,
    ICachedCustomerService cachedCustomerService,
    TimeProvider timeProvider) : ICustomerService
{
    public async Task<Shared.Models.Customer> GetByIdAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            throw new CustomerNotFound();
        }

        if (!ignoreAuthorizationCheck)
        {
            var (_, callingCustomer) = await cachedCustomerService.GetAsync(cancellationToken);

            var askingCustomerOrganizationIds = (await repositoryFactory.OrganizationRepository
                    .GetByCustomerIdAsync(customer.Id, cancellationToken))
                .Select(item => item.Id)
                .ToList();
            var callingCustomerOrganizationIds = (await repositoryFactory.OrganizationRepository
                    .GetByCustomerIdAsync(callingCustomer.Id, cancellationToken))
                .Select(item => item.Id)
                .ToList();

            if (!askingCustomerOrganizationIds.Any(id => callingCustomerOrganizationIds.Contains(id)))
            {
                throw new Unauthorized();
            }
        }

        return mapper.MapTo(customer);
    }

    public async Task<Shared.Models.Customer> GetMeAsync(bool addCustomerIfNotExist, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.GetVerifiableToken());

        var (_, customer) = await cachedCustomerService.GetNullableAsync(cancellationToken);
        if (customer is not null)
        {
            return mapper.MapTo(customer);
        }

        if (!addCustomerIfNotExist)
        {
            throw new CustomerNotFound();
        }

        return await AddAsync(mapper.MapTo(), true, cancellationToken);
    }

    public async Task<(bool, Shared.Models.Customer?)> AnyCustomerExistByVerifiableTokenAsync(
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, cancellationToken);
        return customer is null ? (false, null) : (true, mapper.MapTo(customer));
    }

    public async Task<(bool, Shared.Models.Customer?)> AnyCustomerExistByEmailAsync(
        string email,
        CancellationToken cancellationToken)
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

        var defaultLocations = new List<Location>();
        foreach (var location in customer.DefaultLocations)
        {
            var organization = location.Organization is null
                ? null
                : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(location.Organization.Id, cancellationToken);

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
                : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(team.Organization.Id, cancellationToken);

            defaultTeams.Add(await repositoryFactory.TeamRepository.UpsertNakedAsync(team.Id, organization, cancellationToken));
        }

        var preferredDesks = new List<Desk>();
        foreach (var desk in customer.PreferredDesks)
        {
            var location = await repositoryFactory.LocationRepository.UpsertNakedAsync(desk.Location.Id, null, cancellationToken);
            preferredDesks.Add(await repositoryFactory.DeskRepository.UpsertNakedAsync(desk.Id, location, cancellationToken));
        }

        var preferredRooms = new List<Room>();
        foreach (var room in customer.PreferredRooms)
        {
            var location = await repositoryFactory.LocationRepository.UpsertNakedAsync(room.Location.Id, null, cancellationToken);
            preferredRooms.Add(await repositoryFactory.RoomRepository.UpsertNakedAsync(room.Id, location, cancellationToken));
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
                defaultLocations,
                defaultTeams,
                preferredDesks,
                preferredRooms,
                preferredOrganizationTags);

            identities.ForEach(identity => identity.Customer = existingCustomer);
            repositoryFactory.IdentityRepository.AddRange(identities);
            existingCustomer.Identities = identities;
            customer = mapper.MapTo(repositoryFactory.CustomerRepository.Add(existingCustomer));
        }
        else
        {
            var identity = mapper.MapToIdentity();
            identity.CreatedAt = timeProvider.GetUtcNow();
            existingCustomer.Identities = existingCustomer.Identities.Concat([identity]).ToList();
            customer = mapper.MapTo(repositoryFactory.CustomerRepository.Update(existingCustomer));
        }

        await customerOutboxPublisher.PublishCustomerAsync([customer], repositoryFactory.UnitOfWork, cancellationToken);

        if (sendNewCustomerJoinedEmail)
        {
            await notificationOutboxPublisher.PublishNewCustomerJoinedSubmittedAsync(
                customer,
                repositoryFactory.UnitOfWork,
                cancellationToken);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo((await repositoryFactory.CustomerRepository.GetByIdAsync(customer.Id, cancellationToken))!);
    }

    public async Task<Shared.Models.Customer> AddIdentityAsync(Identity identity, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Customer.Id);
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Id);

        var existingCustomer = await repositoryFactory.CustomerRepository.GetByIdAsync(identity.Customer.Id, cancellationToken);
        if (existingCustomer is null)
        {
            throw new CustomerNotFound();
        }

        if (existingCustomer.Identities.All(item => item.Id != identity.Id))
        {
            await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

            var identityToAdd = mapper.MapTo(identity, existingCustomer);
            repositoryFactory.IdentityRepository.Add(identityToAdd);
            existingCustomer.Identities = existingCustomer.Identities.Concat([identityToAdd]).ToList();

            await customerOutboxPublisher.PublishCustomerAsync(
                [mapper.MapTo(repositoryFactory.CustomerRepository.Update(existingCustomer))],
                repositoryFactory.UnitOfWork,
                cancellationToken);

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        return mapper.MapTo((await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(identity.Id, cancellationToken))!);
    }

    public async Task<Shared.Models.Customer> UpdateIdentityAsync(Identity identity, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Customer.Id);
        ArgumentException.ThrowIfNullOrWhiteSpace(identity.Id);

        var existingCustomer = await repositoryFactory.CustomerRepository.GetByIdAsync(identity.Customer.Id, cancellationToken);
        if (existingCustomer is null)
        {
            throw new CustomerNotFound();
        }

        var matchingIdentityToUpdate = existingCustomer.Identities.First(item => item.Id == identity.Id);
        var identityChanged = identity.Email != matchingIdentityToUpdate.Email || identity.EmailVerified != matchingIdentityToUpdate.EmailVerified;

        if (identityChanged)
        {
            await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

            var identityToUpdate = mapper.MergeTo(identity, matchingIdentityToUpdate, existingCustomer);
            repositoryFactory.IdentityRepository.Update(identityToUpdate);

            await customerOutboxPublisher.PublishCustomerAsync(
                [mapper.MapTo(repositoryFactory.CustomerRepository.Update(existingCustomer))],
                repositoryFactory.UnitOfWork,
                cancellationToken);

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        return mapper.MapTo((await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(identity.Id, cancellationToken))!);
    }
}
