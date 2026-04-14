using Enterprise.Shared.Accounting.Configurations;
using Microsoft.Extensions.Logging;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Client;
using SdkXeroConfiguration = Xero.NetStandard.OAuth2.Config.XeroConfiguration;

namespace Enterprise.Shared.Accounting;

public interface IXeroSdkClientFactory
{
    XeroClient CreateClient(Uri? callbackUri = null);
    AccountingApi CreateAccountingApi();
    IdentityApi CreateIdentityApi();
}

public sealed class XeroSdkClientFactory(XeroConfiguration xeroConfiguration, ILogger<XeroSdkClientFactory> logger) : IXeroSdkClientFactory
{
    public XeroClient CreateClient(Uri? callbackUri = null)
    {
        logger.LogDebug("Creating Xero client. CallbackUriConfigured={CallbackUriConfigured}", callbackUri is not null);

        return new XeroClient(new SdkXeroConfiguration
        {
            ClientId = xeroConfiguration.ClientId,
            ClientSecret = xeroConfiguration.ClientSecret,
            Scope = xeroConfiguration.Scopes,
            CallbackUri = callbackUri
        });
    }

    public AccountingApi CreateAccountingApi()
    {
        logger.LogDebug("Creating Xero AccountingApi client");
        return new AccountingApi();
    }

    public IdentityApi CreateIdentityApi()
    {
        logger.LogDebug("Creating Xero IdentityApi client");
        return new IdentityApi();
    }
}
