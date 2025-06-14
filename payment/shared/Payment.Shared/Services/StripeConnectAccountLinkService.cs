using Api.Shared.Services;
using Api.Shared.Services.OpenApi.Skedular.Payment.V1;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Random;
using Flurl;
using Microsoft.AspNetCore.Mvc;
using Payment.Shared.Database.Entities;
using Payment.Shared.Repositories;
using Stripe;

namespace Payment.Shared.Services;

public interface IStripeConnectAccountLinkService
{
    Task<(StripeConnectAccountRefreshCode, string)> CreateLinkAsync(
        string id,
        string redirectUrl,
        StripeConnectAccount accountEntity,
        CancellationToken cancellationToken);
}

public class StripeConnectAccountLinkService(
    ApplicationConfiguration applicationConfiguration,
    IRandomHelper randomHelper,
    IRepositoryFactory repositoryFactory,
    ICreatable<AccountLink, AccountLinkCreateOptions> accountLinkCreateService) : IStripeConnectAccountLinkService
{
    private readonly Lazy<string> _refreshLinkBaseUrl = new(() =>
    {
        var method = typeof(PaymentControllerBase).GetMethod(nameof(PaymentControllerBase.RefreshOrganizationStripeConnectAccountOnboarding));
        ArgumentNullException.ThrowIfNull(method);

        var routeAttribute = method.GetCustomAttributes(typeof(RouteAttribute), true).Cast<RouteAttribute>().First();
        ArgumentNullException.ThrowIfNull(routeAttribute);

        return routeAttribute.Template;
    });

    public async Task<(StripeConnectAccountRefreshCode, string)> CreateLinkAsync(
        string id,
        string redirectUrl,
        StripeConnectAccount accountEntity,
        CancellationToken cancellationToken)
    {
        var code = randomHelper.Generate(size: Constants.MaxStripeConnectAccountRefreshCodeLength);
        var accountLink = await accountLinkCreateService.CreateAsync(
            new AccountLinkCreateOptions
            {
                Account = id,
                RefreshUrl = Url.Combine(applicationConfiguration.ApiBaseDomain, _refreshLinkBaseUrl.Value).SetQueryParam("code", code),
                ReturnUrl = redirectUrl,
                Type = "account_onboarding"
            },
            new RequestOptions(),
            cancellationToken);

        var accountRefreshCodeEntity = new StripeConnectAccountRefreshCode
        {
            Id = randomHelper.Generate(), Code = code, RedirectUrl = redirectUrl, StripeConnectAccount = accountEntity
        };

        _ = repositoryFactory.StripeConnectAccountRefreshCodeRepository.Add(accountRefreshCodeEntity);

        return (accountRefreshCodeEntity, accountLink.Url);
    }
}
