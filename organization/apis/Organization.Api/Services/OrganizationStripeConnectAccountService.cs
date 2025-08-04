using System.Net;
using Api.Shared.Services;
using Api.Shared.Services.OpenApi.Skedular.Organization.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using Flurl;
using HotChocolate.Types.Pagination;
using Microsoft.AspNetCore.Mvc;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services;
using Stripe;
using Customer = Organization.Shared.Models.Customer;
using OrganizationStripeConnectAccountAuthorization = Organization.Shared.Database.Entities.OrganizationStripeConnectAccountAuthorization;
using StripeConfiguration = Enterprise.Shared.Payment.Configurations.StripeConfiguration;

namespace Organization.Api.Services;

public interface IOrganizationStripeConnectAccountService
{
    Task<OrganizationStripeConnectAccount> AddAsync(
        string? id,
        string organizationId,
        string nickname,
        string redirectUrl,
        CancellationToken cancellationToken);

    Task<OrganizationStripeConnectAccount> UpdateAsync(string id, string nickname, CancellationToken cancellationToken);
    Task<OrganizationStripeConnectAccount> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<OrganizationStripeConnectAccount>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<OrganizationStripeConnectAccount> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<string> GetNewOnboardingUrlAsync(string code, CancellationToken cancellationToken);
    Task<OrganizationStripeConnectAccount> SetAsDefaultAsync(string id, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<OrganizationStripeConnectAccount>>, int)> GetPaginatedAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationStripeConnectAccountSearchCriteria searchCriteria,
        ICollection<OrganizationStripeConnectAccountOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<Uri> ConnectExistingAccountAsync(string code, string scope, string state, CancellationToken cancellationToken);
    Uri GetStripeAuthorizeExistingConnectAccountUrl(string organizationId);
}

public class OrganizationStripeConnectAccountService(
    ApplicationConfiguration applicationConfiguration,
    StripeConfiguration stripeConfiguration,
    IDbTransactionBuilder transactionBuilder,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    ICreatable<Account, AccountCreateOptions> accountCreateService,
    IRetrievable<Account, AccountGetOptions> accountGetOption,
    ICachedCustomerService cachedCustomerService,
    IMapper mapper,
    IRandomHelper randomHelper,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IOrganizationStripeConnectAccountLinkService organizationStripeConnectAccountLinkService,
    ICreatable<OAuthToken, OAuthTokenCreateOptions> oauthTokenCreateService) : IOrganizationStripeConnectAccountService
{
    private readonly Lazy<string> _stripeConnectAccountOAuthCallbackBaseUrl = new(() =>
    {
        var method = typeof(OrganizationControllerBase).GetMethod(nameof(OrganizationControllerBase.StripeConnectAccountOAuthCallback));
        ArgumentNullException.ThrowIfNull(method);

        var routeAttribute = method.GetCustomAttributes(typeof(RouteAttribute), true).Cast<RouteAttribute>().First();
        ArgumentNullException.ThrowIfNull(routeAttribute);

        return routeAttribute.Template;
    });

    public async Task<OrganizationStripeConnectAccount> AddAsync(
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
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanManageStripeConnectAccount(organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        if (!string.IsNullOrWhiteSpace(id))
        {
            var existingAccount = await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByIdAsync(id, cancellationToken);
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
        var accountEntity = mapper.MapTo(stripeConnectAccount, id, nickname, organization.OrganizationStripeConnectAccounts.Count == 0, organization);
        var (accountRefreshCodeEntity, url) = await organizationStripeConnectAccountLinkService.CreateLinkAsync(
            stripeConnectAccount.Id,
            redirectUrl,
            accountEntity,
            cancellationToken);
        accountEntity.OnboardingUrl = url;

        _ = repositoryFactory.OrganizationStripeConnectAccountRefreshCodeRepository.Add(accountRefreshCodeEntity);
        var account = repositoryFactory.OrganizationStripeConnectAccountRepository.Add(accountEntity);
        var mappedAccount = mapper.MapTo(account);

        organizationOutboxPublisher.PublishOrganizations(
            [mapper.MapTo(organization, GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))],
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedAccount;
    }

    public async Task<OrganizationStripeConnectAccount> UpdateAsync(string id, string nickname, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(nickname);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var account = await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByIdAsync(id, cancellationToken) ??
                      throw new OrganizationStripeConnectAccountNotFound();

        return await UpdateInternalAsync(nickname, account, customer, cancellationToken);
    }

    public async Task<OrganizationStripeConnectAccount> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var account = await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByIdAsync(id, cancellationToken) ??
                      throw new OrganizationStripeConnectAccountNotFound();
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdAsync(account.Organization.Id, cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanManageStripeConnectAccount(existingOrganization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        account = repositoryFactory.OrganizationStripeConnectAccountRepository.Remove(account);
        var deletedAccount = mapper.MapTo(account);

        organizationOutboxPublisher.PublishOrganizations(
            [mapper.MapTo(existingOrganization, GetStripeAuthorizeExistingConnectAccountUrl(existingOrganization.Id))], repositoryFactory.UnitOfWork);

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
        var organizationIds = accounts.Select(item => item.Id).ToList();
        var existingOrganizations = await repositoryFactory.OrganizationRepository.GetByIdsAsync(organizationIds, cancellationToken);

        if (existingOrganizations.Any(existingOrganization =>
                !organizationAuthorizationService.CanManageStripeConnectAccount(existingOrganization, customer)))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.OrganizationStripeConnectAccountRepository.RemoveRange(accounts);
        var deletedAccounts = accounts.Select(mapper.MapTo).ToList();

        organizationOutboxPublisher.PublishOrganizations(
            existingOrganizations.Select(item => mapper.MapTo(item, GetStripeAuthorizeExistingConnectAccountUrl(item.Id))),
            repositoryFactory.UnitOfWork);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedAccounts;
    }

    public async Task<OrganizationStripeConnectAccount> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var account = await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByIdAsync(id, cancellationToken) ??
                      throw new OrganizationStripeConnectAccountNotFound();
        var existingOrganizations = await repositoryFactory.OrganizationRepository.GetByIdAsync(account.Organization.Id, cancellationToken) ??
                                    throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanViewStripeConnectAccount(existingOrganizations, customer))
        {
            throw new UnauthorizedAccessException();
        }

        account = await ReSyncOnboardingCompletedStateAsync(account, existingOrganizations, cancellationToken);

        return mapper.MapTo(account);
    }

    public async Task<string> GetNewOnboardingUrlAsync(string code, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        var accountRefreshCode =
            await repositoryFactory.OrganizationStripeConnectAccountRefreshCodeRepository.GetByCodeAsync(code, cancellationToken) ??
            throw new OrganizationStripeConnectAccountRefreshCodeNotFound();
        var (accountRefreshCodeEntity, url) = await organizationStripeConnectAccountLinkService.CreateLinkAsync(
            accountRefreshCode.OrganizationStripeConnectAccount.StripeAccountId,
            accountRefreshCode.RedirectUrl,
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

    public async Task<OrganizationStripeConnectAccount> SetAsDefaultAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var account = await repositoryFactory.OrganizationStripeConnectAccountRepository.GetByIdAsync(id, cancellationToken) ??
                      throw new OrganizationStripeConnectAccountNotFound();
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdAsync(account.Organization.Id, cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanManageStripeConnectAccount(existingOrganization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        foreach (var item in existingOrganization.OrganizationStripeConnectAccounts.Where(item => item.Id != id))
        {
            item.IsDefault = false;
            repositoryFactory.OrganizationStripeConnectAccountRepository.Update(item);
        }

        account.IsDefault = true;
        account = repositoryFactory.OrganizationStripeConnectAccountRepository.Update(account);
        var mappedAccount = mapper.MapTo(account);

        organizationOutboxPublisher.PublishOrganizations(
            [mapper.MapTo(existingOrganization, GetStripeAuthorizeExistingConnectAccountUrl(existingOrganization.Id))], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedAccount;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<OrganizationStripeConnectAccount>>, int)> GetPaginatedAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationStripeConnectAccountSearchCriteria searchCriteria,
        ICollection<OrganizationStripeConnectAccountOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchCriteria.OrganizationId);

        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(searchCriteria.OrganizationId, cancellationToken) ??
                           throw new OrganizationNotFound();

        if (!ignoreAuthorizationCheck)
        {
            var customer = await cachedCustomerService.GetAsync(cancellationToken);
            if (!organizationAuthorizationService.CanViewStripeConnectAccount(organization, customer))
            {
                throw new UnauthorizedAccessException();
            }
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.OrganizationStripeConnectAccountRepository.GetPaginatedAccountsAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        foreach (var account in edges.Select(item => item.Node))
        {
            await ReSyncOnboardingCompletedStateAsync(account, organization, cancellationToken);
        }

        var mappedAccounts = edges.Select(edge => new Edge<OrganizationStripeConnectAccount>(mapper.MapTo(edge.Node), edge.Cursor)).ToList();

        return (paginatedInfo, mappedAccounts, totalCount);
    }

    public async Task<Uri> ConnectExistingAccountAsync(string code, string scope, string state, CancellationToken cancellationToken)
    {
        if (scope != "read_write")
        {
            throw new InvalidOperationException($"scope {scope} is not acceptable, must be read_write");
        }

        var organizationId = state;
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken) ??
                           throw new OrganizationNotFound();

        var oauthToken = await oauthTokenCreateService.CreateAsync(
            new OAuthTokenCreateOptions
            {
                GrantType = "authorization_code", Code = code, Scope = scope, ClientSecret = stripeConfiguration.SecretKey
            },
            new RequestOptions(),
            cancellationToken);
        ArgumentNullException.ThrowIfNull(oauthToken);

        var stripeAccountId = oauthToken.StripeUserId;

        var stripeConnectAccount = await accountGetOption.GetAsync(
            stripeAccountId,
            new AccountGetOptions(),
            new RequestOptions { StripeAccount = stripeAccountId },
            cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var accountEntity = organization.OrganizationStripeConnectAccounts.FirstOrDefault(item => item.StripeAccountId == stripeAccountId);
        if (accountEntity is null)
        {
            accountEntity = mapper.MergeTo(stripeConnectAccount,
                new Shared.Database.Entities.OrganizationStripeConnectAccount
                {
                    Id = randomHelper.Generate(),
                    IsDefault = organization.OrganizationStripeConnectAccounts.All(item => !item.IsDefault),
                    Name = "no name set yet!!!",
                    Organization = organization,
                    OnboardingUrl = string.Empty
                });
            _ = repositoryFactory.OrganizationStripeConnectAccountRepository.Add(accountEntity);
        }
        else
        {
            accountEntity = mapper.MergeTo(stripeConnectAccount, accountEntity);
            _ = repositoryFactory.OrganizationStripeConnectAccountRepository.Update(accountEntity);
        }

        organizationOutboxPublisher.PublishOrganizations(
            [mapper.MapTo(organization, GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new Uri(Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "organizations", organizationId, "setup-marketplace"));
    }

    public Uri GetStripeAuthorizeExistingConnectAccountUrl(string organizationId) =>
        new(
            Url.Combine("https://connect.stripe.com", "oauth", "authorize")
                .SetQueryParam("response_type", "code")
                .SetQueryParam("client_id", stripeConfiguration.OAuthClientId)
                .SetQueryParam("scope", "read_write")
                .SetQueryParam("state", organizationId)
                .SetQueryParam(
                    "redirect_uri",
                    Url.Combine(applicationConfiguration.ApiBaseDomain.ToString(), _stripeConnectAccountOAuthCallbackBaseUrl.Value)));

    private async Task<OrganizationStripeConnectAccount> UpdateInternalAsync(
        string nickname,
        Shared.Database.Entities.OrganizationStripeConnectAccount account,
        Customer customer,
        CancellationToken cancellationToken)
    {
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdAsync(account.Organization.Id, cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanManageStripeConnectAccount(existingOrganization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        account.Name = nickname;
        account = repositoryFactory.OrganizationStripeConnectAccountRepository.Update(account);
        var mappedAccount = mapper.MapTo(account);

        organizationOutboxPublisher.PublishOrganizations(
            [mapper.MapTo(existingOrganization, GetStripeAuthorizeExistingConnectAccountUrl(existingOrganization.Id))], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedAccount;
    }

    private async Task<Shared.Database.Entities.OrganizationStripeConnectAccount> ReSyncOnboardingCompletedStateAsync(
        Shared.Database.Entities.OrganizationStripeConnectAccount account,
        Shared.Database.Entities.Organization organization,
        CancellationToken cancellationToken)
    {
        if (account.IsOnboardingCompleted())
        {
            return account;
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        try
        {
            var stripeConnectAccount = await accountGetOption.GetAsync(
                account.StripeAccountId,
                new AccountGetOptions(),
                new RequestOptions { StripeAccount = account.StripeAccountId },
                cancellationToken);

            account = mapper.MergeTo(stripeConnectAccount, account);

            if (account.OrganizationStripeConnectAccountAuthorization is null)
            {
                account.OrganizationStripeConnectAccountAuthorization =
                    repositoryFactory.OrganizationStripeConnectAccountAuthorizationRepository.Add(
                        new OrganizationStripeConnectAccountAuthorization
                        {
                            Id = randomHelper.Generate(), IsAuthorized = true, OrganizationStripeConnectAccount = account
                        });
            }
            else
            {
                account.OrganizationStripeConnectAccountAuthorization.IsAuthorized = true;
            }
        }
        catch (StripeException ex)
        {
            // Check for a specific unauthorized error
            if (ex.HttpStatusCode != HttpStatusCode.Forbidden && ex.StripeError?.Code != "account_permission_error")
            {
                throw;
            }

            if (account.OrganizationStripeConnectAccountAuthorization is null)
            {
                account.OrganizationStripeConnectAccountAuthorization =
                    repositoryFactory.OrganizationStripeConnectAccountAuthorizationRepository.Add(
                        new OrganizationStripeConnectAccountAuthorization
                        {
                            Id = randomHelper.Generate(), IsAuthorized = false, OrganizationStripeConnectAccount = account
                        });
            }
            else
            {
                account.OrganizationStripeConnectAccountAuthorization.IsAuthorized = false;
            }

            return account;
        }

        account = repositoryFactory.OrganizationStripeConnectAccountRepository.Update(account);

        organizationOutboxPublisher.PublishOrganizations(
            [mapper.MapTo(organization, GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))],
            repositoryFactory.UnitOfWork);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return account;
    }
}
