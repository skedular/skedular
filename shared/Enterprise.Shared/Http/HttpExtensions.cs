using Microsoft.AspNetCore.Http;

namespace Enterprise.Shared.Http;

public static class HttpExtensions
{
    public static CancellationToken GetCancellationToken(this HttpContext? httpContext) =>
        httpContext?.RequestAborted ?? CancellationToken.None;

    public static CancellationToken GetCancellationToken(this IHttpContextAccessor httpContextAccessor) =>
        httpContextAccessor.HttpContext.GetCancellationToken();
}
