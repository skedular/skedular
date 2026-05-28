using Enterprise.Shared.Context;
using Enterprise.Shared.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Security.Token;

public class SecurityContextEnricherMiddleware(
    RequestDelegate next,
    IEnumerable<ITokenService> tokenServices,
    ILogger<SecurityContextEnricherMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext, IContext context)
    {
        var cancellationToken = httpContext.GetCancellationToken();
        var splitToken = httpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ");
        logger.LogDebug("Processing security context enrichment. HasBearerToken={HasBearerToken}", splitToken is ["Bearer", _]);

        if (splitToken is ["Bearer", _])
        {
            var token = splitToken[1];
            logger.LogDebug("Dispatching bearer token verification to token services. ServiceCount={ServiceCount}", tokenServices.Count());
            await Task.WhenAll(tokenServices.Select(tokenService => tokenService.VerifyTokenAsync(token, cancellationToken)));
            logger.LogInformation("Completed bearer token verification across configured token services");
        }

        await next(httpContext);
    }
}
