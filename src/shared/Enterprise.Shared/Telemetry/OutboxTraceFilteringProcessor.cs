using System.Diagnostics;
using Enterprise.Shared.Database;
using OpenTelemetry;

namespace Enterprise.Shared.Telemetry;

public class OutboxTraceFilteringProcessor(BaseProcessor<Activity> innerProcessor) : BaseProcessor<Activity>
{
    public override void OnStart(Activity data) => innerProcessor.OnStart(data);

    public override void OnEnd(Activity data)
    {
        if (OutboxTelemetryFilter.ShouldTraceActivity(data))
        {
            innerProcessor.OnEnd(data);
        }
    }

    protected override bool OnShutdown(int timeoutMilliseconds) => innerProcessor.Shutdown(timeoutMilliseconds);

    protected override bool OnForceFlush(int timeoutMilliseconds) => innerProcessor.ForceFlush(timeoutMilliseconds);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            innerProcessor.Dispose();
        }

        base.Dispose(disposing);
    }
}
