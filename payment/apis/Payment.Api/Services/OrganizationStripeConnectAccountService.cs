using Api.Shared;
using Api.Shared.Services.OpenApi.Skedular.Payment.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using Flurl;
using HotChocolate.Types.Pagination;
using Microsoft.AspNetCore.Mvc;
using Payment.Api.Mappers;
using Payment.Api.Services.Authorization;
using Payment.Shared.Models;
using Payment.Shared.Publishers;
using Payment.Shared.Repositories;
using Stripe;
using OrganizationStripeConnectAccountRefreshCode = Payment.Shared.Database.Entities.OrganizationStripeConnectAccountRefreshCode;

namespace Payment.Api.Services;

public interface IOrganizationStripeConnectAccountService
{
    Task<OrganizationStripeConnectAccount> AddAsync(string organizationId, string nickname, CancellationToken cancellationToken);
    Task<OrganizationStripeConnectAccount> UpdateAsync(string id, string nickname, CancellationToken cancellationToken);
    Task<OrganizationStripeConnectAccount> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<OrganizationStripeConnectAccount>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<OrganizationStripeConnectAccount?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<string> GetNewOnboardingUrlAsync(string code, CancellationToken cancellationToken);
    Task ProcessStripeEventAsync(Account stripeAccount, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<OrganizationStripeConnectAccount>>, int )> GetPaginatedTeamsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationStripeConnectAccountSearchCriteria searchCriteria,
        ICollection<OrganizationStripeConnectAccountOrder> orderByFields,
        CancellationToken cancellationToken);
}

public class OrganizationStripeConnectAccountService(
    ApplicationConfiguration applicationConfiguration,
    IDbTransactionBuilder transactionBuilder,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    ICreatable<Account, AccountCreateOptions> accountCreateService,
    ICreatable<AccountLink, AccountLinkCreateOptions> accountLinkCreateService,
    ICachedCustomerService cachedCustomerService,
    IMapper mapper,
    IPaymentOutboxPublisher paymentOutboxPublisher,
    TimeProvider timeProvider,
    IRandomHelper randomHelper) : IOrganizationStripeConnectAccountService
{
    private readonly Lazy<string> _refreshLinkBaseUrl = new(() =>
    {
        var method = typeof(PaymentControllerBase).GetMethod(nameof(PaymentControllerBase.RefreshOrganizationStripeConnectAccountOnboarding));
        ArgumentNullException.ThrowIfNull(method);

        var routeAttribute = method.GetCustomAttributes(typeof(RouteAttribute), true).Cast<RouteAttribute>().First();
        ArgumentNullException.ThrowIfNull(routeAttribute);

        return routeAttribute.Template;
    });

    public async Task<OrganizationStripeConnectAccount> AddAsync(
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

        if (string.IsNullOrWhiteSpace(organization.ContactEmail))
        {
            throw new OrganizationContactEmailNotSet();
        }

        if (string.IsNullOrWhiteSpace(organization.ContactPhone))
        {
            throw new OrganizationContactPhoneNotSet();
        }

        if (organization.PhysicalAddress is null)
        {
            throw new OrganizationPhysicalAddressNotSet();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var randomRefreshCode = randomHelper.Generate(size: Constants.MaxStripeConnectAccountRefreshCodeLength);
        var stripeConnectAccount =
            await accountCreateService.CreateAsync(mapper.MapTo(organization, nickname), new RequestOptions(), cancellationToken);
        var strAccountLink = await CreateLinkAsync(stripeConnectAccount.Id, organization.Id, randomRefreshCode, cancellationToken);

        var accountEntity = mapper.MapTo(stripeConnectAccount, nickname, organization);
        accountEntity.OnboardingUrl = strAccountLink.Url;

        var accountRefreshCodeEntity = new OrganizationStripeConnectAccountRefreshCode
        {
            Id = randomHelper.Generate(), Code = randomRefreshCode, OrganizationStripeConnectAccount = accountEntity
        };

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

        var randomRefreshCode = randomHelper.Generate(size: Constants.MaxStripeConnectAccountRefreshCodeLength);
        var strAccountLink = await CreateLinkAsync(
            accountRefreshCode.OrganizationStripeConnectAccount.Id,
            accountRefreshCode.OrganizationStripeConnectAccount.Organization.Id,
            randomRefreshCode,
            cancellationToken);

        accountRefreshCode.OrganizationStripeConnectAccount.OnboardingUrl = strAccountLink.Url;

        var accountRefreshCodeEntity = new OrganizationStripeConnectAccountRefreshCode
        {
            Id = randomHelper.Generate(),
            Code = randomRefreshCode,
            OrganizationStripeConnectAccount = accountRefreshCode.OrganizationStripeConnectAccount
        };

        _ = repositoryFactory.OrganizationStripeConnectAccountRefreshCodeRepository.Remove(accountRefreshCode);
        _ = repositoryFactory.OrganizationStripeConnectAccountRefreshCodeRepository.Add(accountRefreshCodeEntity);
        accountRefreshCode.OrganizationStripeConnectAccount =
            repositoryFactory.OrganizationStripeConnectAccountRepository.Update(accountRefreshCode.OrganizationStripeConnectAccount);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return accountRefreshCode.OrganizationStripeConnectAccount.OnboardingUrl;
    }

    public async Task ProcessStripeEventAsync(Account stripeAccount, CancellationToken cancellationToken)
    {
        var account = await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByIdAsync(stripeAccount.Id, cancellationToken);
        if (account is null)
        {
            return;
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        account = mapper.MergeTo(stripeAccount, account);

        account.OnboardingCompletedAt = timeProvider.GetUtcNow();
        account = repositoryFactory.OrganizationStripeConnectAccountRepository.Update(account);
        var mappedAccount = mapper.MapTo(account);

        paymentOutboxPublisher.PublishOrganizationStripeConnectAccounts([mappedAccount], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
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

    private async Task<AccountLink>
        CreateLinkAsync(string id, string organizationId, string randomRefreshCode, CancellationToken cancellationToken) =>
        await accountLinkCreateService.CreateAsync(
            new AccountLinkCreateOptions
            {
                Account = id,
                RefreshUrl =
                    Url.Combine(applicationConfiguration.ApiBaseDomain, _refreshLinkBaseUrl.Value).SetQueryParam("code", randomRefreshCode),
                ReturnUrl = Url.Combine(applicationConfiguration.WebAppBaseDomain, organizationId, "stripe-connect-accounts", id),
                Type = "account_onboarding"
            },
            new RequestOptions(),
            cancellationToken);
}
