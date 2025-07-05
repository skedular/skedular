using Api.Shared.Services.OpenApi.Skedular.Core.V1;
using Microsoft.AspNetCore.Mvc;

namespace Core.Shared;

public class CoreApiHelper
{
    public static string GetPublicCdnFileEndpoint()
    {
        var method = typeof(CoreControllerBase).GetMethod(nameof(CoreControllerBase.GetPublicCdnFile));
        ArgumentNullException.ThrowIfNull(method);

        var routeAttribute = method.GetCustomAttributes(typeof(RouteAttribute), true).Cast<RouteAttribute>().First();
        ArgumentNullException.ThrowIfNull(routeAttribute);

        return routeAttribute.Template[..routeAttribute.Template.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase)];
    }

    public static string GetPrivateFileEndpoint()
    {
        var method = typeof(CoreControllerBase).GetMethod(nameof(CoreControllerBase.GetPrivateFile));
        ArgumentNullException.ThrowIfNull(method);

        var routeAttribute = method.GetCustomAttributes(typeof(RouteAttribute), true).Cast<RouteAttribute>().First();
        ArgumentNullException.ThrowIfNull(routeAttribute);

        return routeAttribute.Template[..routeAttribute.Template.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase)];
    }
}
