using System.Diagnostics;
using System.Net;
using Enterprise.Shared.Infrastructure.ActionResults;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Infrastructure.Filters;

public interface IGlobalHttpExceptionHandler
{
    bool HandleException(ExceptionContext context);
}

public class HttpGlobalExceptionFilter(
    IHostEnvironment env,
    ILogger<HttpGlobalExceptionFilter> logger,
    IGlobalHttpExceptionHandler globalHttpExceptionHandler)
    : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        logger.LogError(new EventId(context.Exception.HResult),
            context.Exception,
            "Exception Message: {Message}", context.Exception.Message);

        if (!globalHttpExceptionHandler.HandleException(context))
        {
            var json = new JsonErrorResponse
            {
                Messages = ["An error occurred.", $"TraceId: {Activity.Current?.TraceId}"],
                DeveloperMessage = env.IsDevelopment() ? context.Exception.ToString() : string.Empty
            };

            context.Result = new InternalServerErrorObjectResult(json);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        context.ExceptionHandled = true;
    }

    public class JsonErrorResponse
    {
        public ICollection<string> Messages { get; set; } = [];
        public string DeveloperMessage { get; set; } = string.Empty;
    }
}
