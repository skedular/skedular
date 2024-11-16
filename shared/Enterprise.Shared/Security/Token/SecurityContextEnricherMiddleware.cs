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
            var propertyBag = (await Task.WhenAll(tokenServices.Select(tokenService =>
                    tokenService.VerifyTokenAsync(token, httpContext?.RequestAborted ?? CancellationToken.None))))
                .FirstOrDefault(item => item is not null);
            // TODO: 20240601 - Morteza: Always copy to property bag, never change the existing instance  
            if (propertyBag is not null)
            {
                context.SetPropertyBag(propertyBag);
            }
        }

        await next(httpContext);
    }
}
