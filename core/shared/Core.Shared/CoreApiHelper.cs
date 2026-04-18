using Api.Shared.Services.OpenApi.Skedular.Core.Core.V1;
using Microsoft.AspNetCore.Mvc;

namespace Core.Shared;

public class CoreApiHelper
{
    private static readonly Lazy<string> s_publicCdnFileEndpoint = new(() =>
    {
        var method = typeof(CoreCoreControllerBase).GetMethod(nameof(CoreCoreControllerBase.GetPublicCdnFile));
        ArgumentNullException.ThrowIfNull(method);

        var routeAttribute = method.GetCustomAttributes(typeof(RouteAttribute), true).Cast<RouteAttribute>().First();
        ArgumentNullException.ThrowIfNull(routeAttribute);

        return routeAttribute.Template[..routeAttribute.Template.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase)];
    });

    private static readonly Lazy<string> s_privateFileEndpoint = new(() =>
    {
        var method = typeof(CoreCoreControllerBase).GetMethod(nameof(CoreCoreControllerBase.GetPrivateFile));
        ArgumentNullException.ThrowIfNull(method);

        var routeAttribute = method.GetCustomAttributes(typeof(RouteAttribute), true).Cast<RouteAttribute>().First();
        ArgumentNullException.ThrowIfNull(routeAttribute);

        return routeAttribute.Template[..routeAttribute.Template.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase)];
    });

    public static string GetPublicCdnFileEndpoint() => s_publicCdnFileEndpoint.Value;

    public static string GetPrivateFileEndpoint() => s_privateFileEndpoint.Value;
}
