using Microsoft.AspNetCore.Http;

namespace Enterprise.Shared.Context;

public class ContextEnricherMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, IContext context)
    {
        var correlationId = httpContext.Request.Headers["X-Correlation-Id"];
        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            context.PropertyBag.AddCorrelationId(correlationId!);
        }

        await next(httpContext);
    }
}
