using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IOrganizationBankAccountService
{
    Task<OrganizationBankAccount> AddAsync(OrganizationBankAccount organizationBankAccount, CancellationToken cancellationToken);
    Task<OrganizationBankAccount> UpdateAsync(OrganizationBankAccount organizationBankAccount, CancellationToken cancellationToken);
    Task<OrganizationBankAccount> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<OrganizationBankAccount>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<OrganizationBankAccount> SetAsDefaultAsync(string id, CancellationToken cancellationToken);
    Task<OrganizationBankAccount> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<OrganizationBankAccount>>, int)> GetPaginatedAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationBankAccountSearchCriteria searchCriteria,
        ICollection<OrganizationBankAccountOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);
}

public class OrganizationBankAccountService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICustomerService customerService,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IMapper mapper) : IOrganizationBankAccountService
{
    public async Task<OrganizationBankAccount> AddAsync(OrganizationBankAccount organizationBankAccount, CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(organizationBankAccount.Id))
        {
            var existingOrganizationBankAccount =
                await repositoryFactory.OrganizationBankAccountRepository.GetByIdAsync(organizationBankAccount.Id, cancellationToken);
            if (existingOrganizationBankAccount is not null)
            {
                return await UpdateInternalAsync(organizationBankAccount, existingOrganizationBankAccount, customer, cancellationToken);
            }
        }
        else
        {
            organizationBankAccount.Id = randomHelper.Generate();
        }

        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                                       organizationBankAccount.Organization.Id,
                                       organizationBankAccount.Organization.UniqueAlphanumericName,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanModify(existingOrganization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        organizationBankAccount.IsDefault = true;

        var mappedResource =
            mapper.MapTo(repositoryFactory.OrganizationBankAccountRepository.Add(mapper.MapTo(organizationBankAccount, existingOrganization)));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return mappedResource;
    }

    public async Task<OrganizationBankAccount> UpdateAsync(OrganizationBankAccount organizationBankAccount, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationBankAccount.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganizationBankAccount =
            await repositoryFactory.OrganizationBankAccountRepository.GetByIdAsync(organizationBankAccount.Id, cancellationToken) ??
            throw new OrganizationBankAccountNotFound();

        return await UpdateInternalAsync(organizationBankAccount, existingOrganizationBankAccount, customer, cancellationToken);
    }

    public async Task<OrganizationBankAccount> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganizationBankAccount = await repositoryFactory.OrganizationBankAccountRepository.GetByIdAsync(id, cancellationToken) ??
                                              throw new ResourceNotFound();
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                                       existingOrganizationBankAccount.Organization.Id,
                                       existingOrganizationBankAccount.Organization.UniqueAlphanumericName,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanModify(existingOrganization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var deletedResource = mapper.MapTo(repositoryFactory.OrganizationBankAccountRepository.Remove(existingOrganizationBankAccount));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedResource;
    }

    public async Task<ICollection<OrganizationBankAccount>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var resources = await repositoryFactory.OrganizationBankAccountRepository.GetByIdsAsync(ids, cancellationToken);
        var organizationIds = resources.Select(item => item.Organization.Id).ToList();
        var existingOrganizations = await repositoryFactory.OrganizationRepository.GetByIdsOrUniqueAlphanumericNamesAsync(
            organizationIds,
            null,
            cancellationToken);

        if (existingOrganizations.Any(existingOrganization => !organizationAuthorizationService.CanModify(existingOrganization, customer)))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.OrganizationBankAccountRepository.RemoveRange(resources);

        var deletedOrganizationBankAccounts = resources.Select(mapper.MapTo).ToList();

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return deletedOrganizationBankAccounts;
    }

    public async Task<OrganizationBankAccount> SetAsDefaultAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganizationBankAccount = await repositoryFactory.OrganizationBankAccountRepository.GetByIdAsync(id, cancellationToken) ??
                                              throw new OrganizationBankAccountNotFound();
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                                       existingOrganizationBankAccount.Organization.Id,
                                       existingOrganizationBankAccount.Organization.UniqueAlphanumericName,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanModify(existingOrganization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        foreach (var item in existingOrganization.OrganizationBankAccounts.Where(item => item.Id != id))
        {
            item.IsDefault = false;
            repositoryFactory.OrganizationBankAccountRepository.Update(item);
        }

        existingOrganizationBankAccount.IsDefault = true;
        var organizationBankAccount = mapper.MapTo(repositoryFactory.OrganizationBankAccountRepository.Update(existingOrganizationBankAccount));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return organizationBankAccount;
    }

    public async Task<OrganizationBankAccount> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var existingOrganizationBankAccount =
            await repositoryFactory.OrganizationBankAccountRepository.GetByIdAsync(id, cancellationToken) ?? throw new ResourceNotFound();
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                                       existingOrganizationBankAccount.Organization.Id,
                                       existingOrganizationBankAccount.Organization.UniqueAlphanumericName,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanView(existingOrganization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        return mapper.MapTo(existingOrganizationBankAccount);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<OrganizationBankAccount>>, int)> GetPaginatedAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationBankAccountSearchCriteria searchCriteria,
        ICollection<OrganizationBankAccountOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                               searchCriteria.OrganizationId,
                               searchCriteria.OrganizationUniqueAlphanumericName,
                               cancellationToken) ??
                           throw new OrganizationNotFound();

        if (!ignoreAuthorizationCheck)
        {
            var customer = await cachedCustomerService.GetAsync(cancellationToken);
            if (!organizationAuthorizationService.CanView(organization, customer))
            {
                throw new UnauthorizedAccessException();
            }
        }

        var (paginatedInfo, edges, totalCount) =
            await repositoryFactory.OrganizationBankAccountRepository.GetPaginatedBankAccountsAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                cancellationToken);

        return (paginatedInfo, edges.Select(item => new Edge<OrganizationBankAccount>(mapper.MapTo(item.Node), item.Cursor)).ToList(), totalCount);
    }

    private async Task<OrganizationBankAccount> UpdateInternalAsync(
        OrganizationBankAccount organizationBankAccount,
        Shared.Database.Entities.OrganizationBankAccount existingOrganizationBankAccount,
        Customer? customer,
        CancellationToken cancellationToken)
    {
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                                       existingOrganizationBankAccount.Organization.Id,
                                       existingOrganizationBankAccount.Organization.UniqueAlphanumericName,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (customer is not null && !organizationAuthorizationService.CanModify(existingOrganization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        organizationBankAccount.IsDefault = existingOrganizationBankAccount.IsDefault;
        existingOrganizationBankAccount = mapper.MergeTo(organizationBankAccount, existingOrganizationBankAccount, existingOrganization);

        organizationBankAccount = mapper.MapTo(repositoryFactory.OrganizationBankAccountRepository.Update(existingOrganizationBankAccount));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return organizationBankAccount;
    }
}
