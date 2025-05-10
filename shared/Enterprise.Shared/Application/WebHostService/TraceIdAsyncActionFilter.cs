using Enterprise.Shared.Telemetry;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Enterprise.Shared.Application.WebHostService;

public class TraceIdAsyncActionFilter(TraceSettings traceSettings, IActivityGetter activityGetter) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await next();

        var activityCurrent = activityGetter.GetCurrent();
        if (traceSettings.EnableTraceParentOnResponseHeader && activityCurrent is not null)
        {
            context.HttpContext.Response.Headers.TraceParent = activityCurrent.Id;
        }
    }
}
