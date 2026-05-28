using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Context;

public class ContextEnricherMiddleware(RequestDelegate next, ILogger<ContextEnricherMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext, IContext context)
    {
        var correlationId = httpContext.Request.Headers["X-Correlation-Id"].ToString();
        logger.LogDebug("Applying correlation id from request. HasCorrelationId={HasCorrelationId}", !string.IsNullOrWhiteSpace(correlationId));
        context.SetCorrelationId(correlationId);

        await next(httpContext);
    }
}
