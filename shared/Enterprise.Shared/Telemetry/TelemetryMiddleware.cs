using System.Diagnostics;
using System.Diagnostics.Metrics;
using Enterprise.Shared.Metrics;
using Microsoft.AspNetCore.Http;

namespace Enterprise.Shared.Telemetry;

public sealed class TelemetryMiddleware : ITaggable<HttpContext>
{
    private readonly Counter<long> _httpRequestsTotalCounter;
    private readonly RequestDelegate _next;
    private volatile float _httpRequestsDuration;

    public TelemetryMiddleware(RequestDelegate next, IOpenTelemetryInstrumentation meters)
    {
        _next = next;
        _httpRequestsTotalCounter = meters.GetCounterByName<long>(MetricNames.HttpTotalRequestsCounter);

        meters.GetObservableGaugeByName(MetricNames.HttpRequestsDurationGauge, () => new Measurement<float>(_httpRequestsDuration));
    }

    public TagList GetTags(HttpContext ctx) => ctx.GetTagListFromHttpContext<TelemetryMiddleware>();

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            await _next(httpContext);
        }
        finally
        {
            stopwatch.Stop();

            _httpRequestsTotalCounter.Add(1, GetTags(httpContext));
            _httpRequestsDuration = stopwatch.ElapsedMilliseconds;
        }
    }
}
