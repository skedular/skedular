using Api.Shared.Services;
using Enterprise.Shared.Database;
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
using StripeConfiguration = Enterprise.Shared.Payment.Configurations.StripeConfiguration;

namespace Payment.Api.Services;

public interface IOrganizationStripeConnectAccountService
{
    Task<StripeConnectAccount> AddAsync(
        string? id,
        string organizationId,
        string nickname,
        string redirectUrl,
        CancellationToken cancellationToken);

    Task<StripeConnectAccount> UpdateAsync(string id, string nickname, CancellationToken cancellationToken);
    Task<StripeConnectAccount> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<StripeConnectAccount>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<StripeConnectAccount?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<string> GetNewOnboardingUrlAsync(string code, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<StripeConnectAccount>>, int )> GetPaginatedAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationStripeConnectAccountSearchCriteria searchCriteria,
        ICollection<OrganizationStripeConnectAccountOrder> orderByFields,
        CancellationToken cancellationToken);
}

public class OrganizationStripeConnectAccountService(
    StripeConfiguration stripeConfiguration,
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
    public async Task<StripeConnectAccount> AddAsync(
        string? id,
        string organizationId,
        string nickname,
        string redirectUrl,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(nickname);
        ArgumentException.ThrowIfNullOrWhiteSpace(redirectUrl);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, false, false, cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanManageStripeConnectAccount(organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        if (string.IsNullOrWhiteSpace(organization.Name))
        {
            throw new OrganizationNameIsInvalid();
        }

        if (!string.IsNullOrWhiteSpace(id))
        {
            var existingAccount = await repositoryFactory.StripeConnectAccountRepository.GetByIdAsync(id, cancellationToken);
            if (existingAccount is not null)
            {
                if (existingAccount.Organization == null || existingAccount.Organization.Id != organizationId)
                {
                    throw new UnauthorizedAccessException();
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
            redirectUrl,
            accountEntity,
            cancellationToken);
        accountEntity.OnboardingUrl = url;

        _ = repositoryFactory.StripeConnectAccountRefreshCodeRepository.Add(accountRefreshCodeEntity);
        var account = repositoryFactory.StripeConnectAccountRepository.Add(accountEntity);
        var mappedAccount = mapper.MapTo(account);

        paymentOutboxPublisher.PublishOrganizationStripeConnectAccounts([mappedAccount], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedAccount;
    }

    public async Task<StripeConnectAccount> UpdateAsync(string id, string nickname, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(nickname);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var account = await repositoryFactory.StripeConnectAccountRepository.GetByIdAsync(id, cancellationToken) ??
                      throw new OrganizationStripeConnectAccountNotFound();

        return await UpdateInternalAsync(nickname, account, customer, cancellationToken);
    }

    public async Task<StripeConnectAccount> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var account = await repositoryFactory.StripeConnectAccountRepository.GetByIdAsync(id, cancellationToken) ??
                      throw new OrganizationStripeConnectAccountNotFound();
        if (account.Organization == null)
        {
            throw new InvalidOperationException();
        }

        if (!organizationAuthorizationService.CanManageStripeConnectAccount(account.Organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        if (stripeConfiguration.RemoveStripeConnectAccountFromStripe)
        {
            await accountDeleteService.DeleteAsync(
                account.StripeAccountId,
                new AccountDeleteOptions(),
                new RequestOptions { IdempotencyKey = account.Id },
                cancellationToken);
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        account = repositoryFactory.StripeConnectAccountRepository.Remove(account);
        var deletedAccount = mapper.MapTo(account);

        paymentOutboxPublisher.PublishOrganizationStripeConnectAccounts([deletedAccount], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return deletedAccount;
    }

    public async Task<ICollection<StripeConnectAccount>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var accounts = await repositoryFactory.StripeConnectAccountRepository.GetByIdsAsync(ids, cancellationToken);

        if (accounts.Any(item => item.Organization == null))
        {
            throw new InvalidOperationException();
        }

        var organizationIds = accounts.Select(item => item.Id).ToList();
        var existingOrganizations = await repositoryFactory.OrganizationRepository.GetByIdsAsync(organizationIds, cancellationToken);

        if (existingOrganizations.Any(existingOrganization =>
                !organizationAuthorizationService.CanManageStripeConnectAccount(existingOrganization, customer)))
        {
            throw new UnauthorizedAccessException();
        }

        if (stripeConfiguration.RemoveStripeConnectAccountFromStripe)
        {
            await Task.WhenAll(
                accounts.Select(item => accountDeleteService.DeleteAsync(
                    item.StripeAccountId,
                    new AccountDeleteOptions(),
                    new RequestOptions { IdempotencyKey = item.Id },
                    cancellationToken)));
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.StripeConnectAccountRepository.RemoveRange(accounts);
        var deletedAccounts = accounts.Select(mapper.MapTo).ToList();

        paymentOutboxPublisher.PublishOrganizationStripeConnectAccounts(deletedAccounts, repositoryFactory.UnitOfWork);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedAccounts;
    }

    public async Task<StripeConnectAccount?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var account = await repositoryFactory.StripeConnectAccountRepository.GetByIdAsync(id, cancellationToken);
        if (account is null)
        {
            return null;
        }

        if (account.Organization == null)
        {
            throw new InvalidOperationException();
        }

        if (!organizationAuthorizationService.CanViewStripeConnectAccount(account.Organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        return mapper.MapTo(account);
    }

    public async Task<string> GetNewOnboardingUrlAsync(string code, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        var accountRefreshCode = await repositoryFactory.StripeConnectAccountRefreshCodeRepository.GetByCodeAsync(code, cancellationToken) ??
                                 throw new OrganizationStripeConnectAccountRefreshCodeNotFound();
        var (accountRefreshCodeEntity, url) = await stripeConnectAccountLinkService.CreateLinkAsync(
            accountRefreshCode.StripeConnectAccount.StripeAccountId,
            accountRefreshCode.RedirectUrl,
            accountRefreshCode.StripeConnectAccount,
            cancellationToken);

        accountRefreshCode.StripeConnectAccount.OnboardingUrl = url;

        _ = repositoryFactory.StripeConnectAccountRefreshCodeRepository.Remove(accountRefreshCode);
        _ = repositoryFactory.StripeConnectAccountRefreshCodeRepository.Add(accountRefreshCodeEntity);
        accountRefreshCode.StripeConnectAccount =
            repositoryFactory.StripeConnectAccountRepository.Update(accountRefreshCode.StripeConnectAccount);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return accountRefreshCode.StripeConnectAccount.OnboardingUrl;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<StripeConnectAccount>>, int)> GetPaginatedAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationStripeConnectAccountSearchCriteria searchCriteria,
        ICollection<OrganizationStripeConnectAccountOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchCriteria.OrganizationId);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(searchCriteria.OrganizationId, false, false, cancellationToken) ??
            throw new OrganizationNotFound();

        if (!organizationAuthorizationService.CanViewStripeConnectAccount(organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.StripeConnectAccountRepository.GetPaginatedAccountsAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        var mappedAccounts = edges.Select(edge => new Edge<StripeConnectAccount>(mapper.MapTo(edge.Node), edge.Cursor)).ToList();

        return (paginatedInfo, mappedAccounts, totalCount);
    }

    private async Task<StripeConnectAccount> UpdateInternalAsync(
        string nickname,
        Shared.Database.Entities.StripeConnectAccount account,
        Customer customer,
        CancellationToken cancellationToken)
    {
        if (account.Organization == null)
        {
            throw new InvalidOperationException();
        }

        if (!organizationAuthorizationService.CanManageStripeConnectAccount(account.Organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        account.Name = nickname;
        account = repositoryFactory.StripeConnectAccountRepository.Update(account);
        var mappedAccount = mapper.MapTo(account);

        paymentOutboxPublisher.PublishOrganizationStripeConnectAccounts([mappedAccount], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedAccount;
    }
}
