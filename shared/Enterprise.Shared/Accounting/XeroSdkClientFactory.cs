using Enterprise.Shared.Accounting.Configurations;
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

public sealed class XeroSdkClientFactory(XeroConfiguration xeroConfiguration) : IXeroSdkClientFactory
{
    public XeroClient CreateClient(Uri? callbackUri = null) =>
        new(new SdkXeroConfiguration
        {
            ClientId = xeroConfiguration.ClientId,
            ClientSecret = xeroConfiguration.ClientSecret,
            Scope = xeroConfiguration.Scopes,
            CallbackUri = callbackUri
        });

    public AccountingApi CreateAccountingApi() => new();
    public IdentityApi CreateIdentityApi() => new();
}
