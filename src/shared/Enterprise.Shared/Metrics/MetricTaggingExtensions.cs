using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Enterprise.Shared.Metrics;

public static class MetricTaggingExtensions
{
    extension(HttpContext ctx)
    {
        public TagList GetTagListFromHttpContext<TClass>() =>
            new()
            {
                { "controller-type", typeof(TClass).Name },
                { "request-path", ctx.Request.Path },
                { "http-scheme", ctx.Request.Scheme },
                { "http-method", ctx.Request.Method },
                { "http-status-code", ctx.Response.StatusCode.ToString() },
            };
    }
}
