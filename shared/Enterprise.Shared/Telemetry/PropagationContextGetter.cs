using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace Enterprise.Shared.Telemetry;

public interface IPropagationContextGetter
{
    PropagationContext? GetPropagationContext();
}

public class PropagationContextGetter(IActivityGetter activityGetter, ILogger<PropagationContextGetter> logger) : IPropagationContextGetter
{
    public PropagationContext? GetPropagationContext()
    {
        var activity = activityGetter.GetCurrent();

        if (activity is null)
        {
            logger.LogDebug("No current activity was available for propagation context extraction");
            return null;
        }

        var baggage = Baggage.SetBaggage(activity.Baggage);
        logger.LogDebug("Resolved propagation context from current activity");

        return new PropagationContext(activity.Context, baggage);
    }
}
