using Microsoft.AspNetCore.Http;

namespace Enterprise.Shared.Context;

public class ContextEnricherMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, IContext context)
    {
        context.SetCorrelationId(httpContext.Request.Headers["X-Correlation-Id"]!);

        await next(httpContext);
    }
}
