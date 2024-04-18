using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Enterprise.Shared.Application.WebHostService;

public class TraceIdAsyncActionFilter(TraceSettings traceSettings) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        await next();

        if (traceSettings.EnableTraceParentOnResponseHeader && Activity.Current is not null)
        {
            context.HttpContext.Response.Headers.TraceParent = Activity.Current.Id;
        }
    }
}
