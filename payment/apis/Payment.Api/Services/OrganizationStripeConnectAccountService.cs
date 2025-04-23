using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Payment.Api.Mappers;
using Payment.Api.Services.Authorization;
using Payment.Shared.Models;
using Payment.Shared.Publishers;
using Payment.Shared.Repositories;
using Payment.Shared.Services;
using Stripe;
using Customer = Payment.Shared.Models.Customer;

namespace Payment.Api.Services;

public interface IOrganizationStripeConnectAccountService
{
    Task<OrganizationStripeConnectAccount> AddAsync(string? id, string organizationId, string nickname, CancellationToken cancellationToken);
    Task<OrganizationStripeConnectAccount> UpdateAsync(string id, string nickname, CancellationToken cancellationToken);
    Task<OrganizationStripeConnectAccount> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<OrganizationStripeConnectAccount>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<OrganizationStripeConnectAccount?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<string> GetNewOnboardingUrlAsync(string code, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<OrganizationStripeConnectAccount>>, int )> GetPaginatedTeamsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationStripeConnectAccountSearchCriteria searchCriteria,
        ICollection<OrganizationStripeConnectAccountOrder> orderByFields,
        CancellationToken cancellationToken);
}

public class OrganizationStripeConnectAccountService(
    IDbTransactionBuilder transactionBuilder,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    ICreatable<Account, AccountCreateOptions> accountCreateService,
    IDeletable<Account, AccountDeleteOptions> accountDeleteService,
    ICachedCustomerService cachedCustomerService,
    IMapper mapper,
    IRandomHelper randomHelper,
    IPaymentOutboxPublisher paymentOutboxPublisher,
    IStripeConnectAccountLinkService stripeConnectAccountLinkService) : IOrganizationStripeConnectAccountService
{
    public async Task<OrganizationStripeConnectAccount> AddAsync(
        string? id,
        string organizationId,
        string nickname,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(nickname);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, false, false, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanManageStripeConnectAccount(organization, customer))
        {
            throw new Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(organization.Name))
        {
            throw new OrganizationNameIsInvalid();
        }

        if (!string.IsNullOrWhiteSpace(id))
        {
            var existingAccount = await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByIdAsync(id, cancellationToken);
            if (existingAccount is not null)
            {
                if (existingAccount.Organization.Id != organizationId)
                {
                    throw new InvalidOperationException();
                }

                return await UpdateInternalAsync(nickname, existingAccount, customer, cancellationToken);
            }
        }
        else
        {
            id = randomHelper.Generate();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var stripeConnectAccount = await accountCreateService.CreateAsync(
            mapper.MapToStripeAccountRequest(organization),
            new RequestOptions { IdempotencyKey = id },
            cancellationToken);
        var accountEntity = mapper.MapTo(stripeConnectAccount, id, nickname, organization);
        var (accountRefreshCodeEntity, url) = await stripeConnectAccountLinkService.CreateLinkAsync(
            stripeConnectAccount.Id,
            organization.Id,
            accountEntity,
            cancellationToken);
        accountEntity.OnboardingUrl = url;

        _ = repositoryFactory.OrganizationStripeConnectAccountRefreshCodeRepository.Add(accountRefreshCodeEntity);
        var account = repositoryFactory.OrganizationStripeConnectAccountRepository.Add(accountEntity);
        var mappedAccount = mapper.MapTo(account);

        paymentOutboxPublisher.PublishOrganizationStripeConnectAccounts([mappedAccount], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedAccount;
    }

    public async Task<OrganizationStripeConnectAccount> UpdateAsync(string id, string nickname, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(nickname);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var account = await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByIdAsync(id, cancellationToken);
        if (account is null)
        {
            throw new OrganizationStripeConnectAccountNotFound();
        }

        return await UpdateInternalAsync(nickname, account, customer, cancellationToken);
    }

    public async Task<OrganizationStripeConnectAccount> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var account = await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByIdAsync(id, cancellationToken);
        if (account is null)
        {
            throw new OrganizationStripeConnectAccountNotFound();
        }

        if (!organizationAuthorizationService.CanManageStripeConnectAccount(account.Organization, customer))
        {
            throw new Unauthorized();
        }

        await accountDeleteService.DeleteAsync(
            account.StripeAccountId,
            new AccountDeleteOptions(),
            new RequestOptions { IdempotencyKey = account.Id },
            cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        account = repositoryFactory.OrganizationStripeConnectAccountRepository.Remove(account);
        var deletedAccount = mapper.MapTo(account);

        paymentOutboxPublisher.PublishOrganizationStripeConnectAccounts([deletedAccount], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return deletedAccount;
    }

    public async Task<ICollection<OrganizationStripeConnectAccount>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var accounts = await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByIdsAsync(ids, cancellationToken);
        var organizationIds = accounts.Select(item => item.Organization.Id).ToList();
        var existingOrganizations = await repositoryFactory.OrganizationRepository.GetByIdsAsync(organizationIds, cancellationToken);

        if (existingOrganizations.Any(existingOrganization =>
                !organizationAuthorizationService.CanManageStripeConnectAccount(existingOrganization, customer)))
        {
            throw new Unauthorized();
        }

        await Task.WhenAll(
            accounts.Select(item => accountDeleteService.DeleteAsync(
                item.StripeAccountId,
                new AccountDeleteOptions(),
                new RequestOptions { IdempotencyKey = item.Id },
                cancellationToken)));

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.OrganizationStripeConnectAccountRepository.RemoveRange(accounts);
        var deletedAccounts = accounts.Select(mapper.MapTo).ToList();

        paymentOutboxPublisher.PublishOrganizationStripeConnectAccounts(deletedAccounts, repositoryFactory.UnitOfWork);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedAccounts;
    }

    public async Task<OrganizationStripeConnectAccount?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var account = await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByIdAsync(id, cancellationToken);
        if (account is null)
        {
            return null;
        }

        if (!organizationAuthorizationService.CanViewStripeConnectAccount(account.Organization, customer))
        {
            throw new Unauthorized();
        }

        return mapper.MapTo(account);
    }

    public async Task<string> GetNewOnboardingUrlAsync(string code, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        var accountRefreshCode =
            await repositoryFactory.OrganizationStripeConnectAccountRefreshCodeRepository.GetByCodeAsync(code, cancellationToken);
        if (accountRefreshCode is null)
        {
            throw new OrganizationStripeConnectAccountRefreshCodeNotFound();
        }

        var (accountRefreshCodeEntity, url) = await stripeConnectAccountLinkService.CreateLinkAsync(
            accountRefreshCode.OrganizationStripeConnectAccount.StripeAccountId,
            accountRefreshCode.OrganizationStripeConnectAccount.Organization.Id,
            accountRefreshCode.OrganizationStripeConnectAccount,
            cancellationToken);

        accountRefreshCode.OrganizationStripeConnectAccount.OnboardingUrl = url;

        _ = repositoryFactory.OrganizationStripeConnectAccountRefreshCodeRepository.Remove(accountRefreshCode);
        _ = repositoryFactory.OrganizationStripeConnectAccountRefreshCodeRepository.Add(accountRefreshCodeEntity);
        accountRefreshCode.OrganizationStripeConnectAccount =
            repositoryFactory.OrganizationStripeConnectAccountRepository.Update(accountRefreshCode.OrganizationStripeConnectAccount);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return accountRefreshCode.OrganizationStripeConnectAccount.OnboardingUrl;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<OrganizationStripeConnectAccount>>, int)> GetPaginatedTeamsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationStripeConnectAccountSearchCriteria searchCriteria,
        ICollection<OrganizationStripeConnectAccountOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchCriteria.OrganizationId);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(searchCriteria.OrganizationId, false, false, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanViewStripeConnectAccount(organization, customer))
        {
            throw new Unauthorized();
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.OrganizationStripeConnectAccountRepository.GetPaginatedAccountsAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        var mappedAccounts = edges.Select(edge => new Edge<OrganizationStripeConnectAccount>(mapper.MapTo(edge.Node), edge.Cursor)).ToList();

        return (paginatedInfo, mappedAccounts, totalCount);
    }

    private async Task<OrganizationStripeConnectAccount> UpdateInternalAsync(
        string nickname,
        Shared.Database.Entities.OrganizationStripeConnectAccount account,
        Customer customer,
        CancellationToken cancellationToken)
    {
        if (!organizationAuthorizationService.CanManageStripeConnectAccount(account.Organization, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        account.Name = nickname;
        account = repositoryFactory.OrganizationStripeConnectAccountRepository.Update(account);
        var mappedAccount = mapper.MapTo(account);

        paymentOutboxPublisher.PublishOrganizationStripeConnectAccounts([mappedAccount], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedAccount;
    }
}
