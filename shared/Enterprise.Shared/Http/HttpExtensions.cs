using Microsoft.AspNetCore.Http;

namespace Enterprise.Shared.Http;

public static class HttpExtensions
{
    extension(HttpContext? httpContext)
    {
        public CancellationToken GetCancellationToken() => httpContext?.RequestAborted ?? CancellationToken.None;
    }

    extension(IHttpContextAccessor httpContextAccessor)
    {
        public CancellationToken GetCancellationToken() => httpContextAccessor.HttpContext.GetCancellationToken();
    }
}
