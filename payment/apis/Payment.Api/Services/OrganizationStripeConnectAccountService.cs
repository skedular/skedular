using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Pagination;
using Flurl;
using HotChocolate.Types.Pagination;
using Payment.Api.Mappers;
using Payment.Api.Services.Authorization;
using Payment.Shared.Models;
using Payment.Shared.Publishers;
using Payment.Shared.Repositories;
using Stripe;

namespace Payment.Api.Services;

public interface IOrganizationStripeConnectAccountService
{
    Task<OrganizationStripeConnectAccount> AddAsync(string organizationId, string name, CancellationToken cancellationToken);
    Task<OrganizationStripeConnectAccount> UpdateAsync(string id, string name, CancellationToken cancellationToken);
    Task<OrganizationStripeConnectAccount> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<OrganizationStripeConnectAccount?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<string> GetNewOnboardingUrlAsync(string id, CancellationToken cancellationToken);
    Task CompleteOnboardAsync(Account stripeAccount, CancellationToken cancellationToken);

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
    TimeProvider timeProvider) : IOrganizationStripeConnectAccountService
{
    public async Task<OrganizationStripeConnectAccount> AddAsync(
        string organizationId,
        string name,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

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

        var stripeConnectAccount = await accountCreateService.CreateAsync(mapper.MapTo(organization), new RequestOptions(), cancellationToken);
        var strAccountLink = await accountLinkCreateService.CreateAsync(
            new AccountLinkCreateOptions
            {
                Account = stripeConnectAccount.Id,
                RefreshUrl = Url.Combine(
                    applicationConfiguration.ApiBaseDomain,
                    $"payment/api/v1/organization-stripe-connect-account/{stripeConnectAccount.Id}/refresh-onboarding-url"),
                ReturnUrl = Url.Combine(
                    applicationConfiguration.ApiBaseDomain,
                    "payment/api/v1/organization-stripe-connect-account/onboarding-completed"),
                Type = "account_onboarding"
            },
            new RequestOptions(),
            cancellationToken);

        var accountEntity = mapper.MapTo(stripeConnectAccount, name, organization);
        accountEntity.OnboardingUrl = strAccountLink.Url;
        var account = repositoryFactory.OrganizationStripeConnectAccountRepository.Add(accountEntity);
        var mappedAccount = mapper.MapTo(account);

        paymentOutboxPublisher.PublishOrganizationStripeConnectAccounts([mappedAccount], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedAccount;
    }

    public async Task<OrganizationStripeConnectAccount> UpdateAsync(string id, string name, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

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

        account.Name = name;
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

    public async Task<string> GetNewOnboardingUrlAsync(string id, CancellationToken cancellationToken)
    {
        var account = await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByIdAsync(id, cancellationToken);
        if (account is null)
        {
            throw new OrganizationStripeConnectAccountNotFound();
        }
        
        var strAccountLink = await accountLinkCreateService.CreateAsync(
            new AccountLinkCreateOptions
            {
                Account = account.Id,
                RefreshUrl = Url.Combine(
                    applicationConfiguration.ApiBaseDomain,
                    $"payment/api/v1/organization-stripe-connect-account/{account.Id}/refresh-onboarding-url"),
                ReturnUrl = Url.Combine(
                    applicationConfiguration.ApiBaseDomain,
                    "payment/api/v1/organization-stripe-connect-account/onboarding-completed"),
                Type = "account_onboarding"
            },
            new RequestOptions(),
            cancellationToken);

        account.OnboardingUrl = strAccountLink.Url;;
        account = repositoryFactory.OrganizationStripeConnectAccountRepository.Update(account);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return account.OnboardingUrl;
    }

    public async Task CompleteOnboardAsync(Account stripeAccount, CancellationToken cancellationToken)
    {
        var account = await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByIdAsync(stripeAccount.Id, cancellationToken);
        if (account is null)
        {
            throw new OrganizationStripeConnectAccountNotFound();
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
}
