using Enterprise.Shared.Context;
using Microsoft.AspNetCore.Http;

namespace Enterprise.Shared.Security.Token;

public class SecurityContextEnricherMiddleware(RequestDelegate next, IEnumerable<ITokenService> tokenServices)
{
    public async Task InvokeAsync(HttpContext httpContext, IContext context)
    {
        var splitToken = httpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ");
        if (splitToken is ["Bearer", _])
        {
            var token = splitToken[1];
            await Task.WhenAll(tokenServices.Select(tokenService =>
                tokenService.VerifyTokenAsync(token, httpContext?.RequestAborted ?? CancellationToken.None)));
        }

        await next(httpContext);
    }
}
