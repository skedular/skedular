using Api.Shared.Services.OpenApi.Skedular.Organization.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Random;
using Flurl;
using Microsoft.AspNetCore.Mvc;
using Organization.Shared.Database.Entities;
using Organization.Shared.Repositories;
using Stripe;

namespace Organization.Shared.Services;

public interface IOrganizationStripeConnectAccountLinkService
{
    Task<(OrganizationStripeConnectAccountRefreshCode, string)> CreateLinkAsync(
        string id,
        string redirectUrl,
        OrganizationStripeConnectAccount accountEntity,
        CancellationToken cancellationToken);
}

public class OrganizationStripeConnectAccountLinkService(
    ApplicationConfiguration applicationConfiguration,
    IRandomHelper randomHelper,
    IRepositoryFactory repositoryFactory,
    ICreatable<AccountLink, AccountLinkCreateOptions> accountLinkCreateService) : IOrganizationStripeConnectAccountLinkService
{
    private readonly Lazy<string> _refreshLinkBaseUrl = new(() =>
    {
        var method = typeof(OrganizationControllerBase).GetMethod(
            nameof(OrganizationControllerBase.RefreshOrganizationStripeConnectAccountOnboarding));
        ArgumentNullException.ThrowIfNull(method);

        var routeAttribute = method.GetCustomAttributes(typeof(RouteAttribute), true).Cast<RouteAttribute>().First();
        ArgumentNullException.ThrowIfNull(routeAttribute);

        return routeAttribute.Template;
    });

    public async Task<(OrganizationStripeConnectAccountRefreshCode, string)> CreateLinkAsync(
        string id,
        string redirectUrl,
        OrganizationStripeConnectAccount accountEntity,
        CancellationToken cancellationToken)
    {
        var code = randomHelper.Generate();
        var accountLink = await accountLinkCreateService.CreateAsync(
            new AccountLinkCreateOptions
            {
                Account = id,
                RefreshUrl =
                    Url.Combine(applicationConfiguration.ApiBaseDomain.ToString(), _refreshLinkBaseUrl.Value).SetQueryParam("code", code),
                ReturnUrl = redirectUrl,
                Type = "account_onboarding"
            },
            new RequestOptions(),
            cancellationToken);

        var accountRefreshCodeEntity = new OrganizationStripeConnectAccountRefreshCode
        {
            Id = randomHelper.Generate(), Code = code, RedirectUrl = redirectUrl, OrganizationStripeConnectAccount = accountEntity
        };

        _ = repositoryFactory.OrganizationStripeConnectAccountRefreshCodeRepository.Add(accountRefreshCodeEntity);

        return (accountRefreshCodeEntity, accountLink.Url);
    }
}
