using OpenTelemetry;
using OpenTelemetry.Context.Propagation;

namespace Enterprise.Shared.Telemetry;

public interface IPropagationContextGetter
{
    PropagationContext? GetPropagationContext();
}

public class PropagationContextGetter(IActivityGetter activityGetter) : IPropagationContextGetter
{
    public PropagationContext? GetPropagationContext()
    {
        var activity = activityGetter.GetCurrent();

        if (activity is null)
        {
            return null;
        }

        var baggage = Baggage.SetBaggage(activity.Baggage);

        return new PropagationContext(activity.Context, baggage);
    }
}
