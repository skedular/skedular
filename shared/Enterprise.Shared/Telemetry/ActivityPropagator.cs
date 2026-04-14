using System.Diagnostics;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Context.Propagation;

namespace Enterprise.Shared.Telemetry;

public interface IActivityPropagator<in T> where T : class
{
    /// <summary>
    ///     Propagate the currently active activity to the destination
    /// </summary>
    /// <param name="destination"></param>
    void PropagateActivity(T destination);

    /// <summary>
    ///     Create and start an activity using incoming headers
    /// </summary>
    /// <param name="location"></param>
    /// <returns>The created activity (may be null)</returns>
    PropagationContext GetActivityPropagationContext(T location);

    Activity? StartActivityFromPropagationContext(
        T location,
        IActivitySource activitySource,
        string activityName,
        ActivityKind kind = ActivityKind.Internal,
        IEnumerable<KeyValuePair<string, object?>>? tags = null);
}

/// <summary>
///     Access and propagate the context using any entity. Requires that entity to have a
///     <see cref="IPropagatorFunctionProvider{T}" />
/// </summary>
/// <typeparam name="T">Entity to use for propagation</typeparam>
public class ActivityPropagator<T>(
    IPropagationContextGetter propagationContextGetter,
    TextMapPropagator textMapPropagator,
    IPropagatorFunctionProvider<T> functionProvider,
    ILogger<ActivityPropagator<T>> logger)
    : IActivityPropagator<T> where T : class
{
    public void PropagateActivity(T destination)
    {
        ArgumentNullException.ThrowIfNull(destination);

        var propagationContext = propagationContextGetter.GetPropagationContext();
        if (propagationContext is null)
        {
            logger.LogDebug("No propagation context available to inject. DestinationType={DestinationType}", typeof(T).Name);
            return;
        }

        logger.LogDebug("Injecting propagation context. DestinationType={DestinationType}", typeof(T).Name);
        textMapPropagator.Inject(propagationContext.Value, destination, functionProvider.Inject);
    }

    public PropagationContext GetActivityPropagationContext(T location)
    {
        logger.LogDebug("Extracting propagation context. LocationType={LocationType}", typeof(T).Name);
        return textMapPropagator.Extract(new PropagationContext(), location, functionProvider.Extract);
    }

    public Activity? StartActivityFromPropagationContext(
        T location,
        IActivitySource activitySource,
        string activityName,
        ActivityKind kind = ActivityKind.Internal,
        IEnumerable<KeyValuePair<string, object?>>? tags = null)
    {
        var propagationContext = GetActivityPropagationContext(location);
        logger.LogDebug("Starting activity from propagation context. ActivityName={ActivityName}, LocationType={LocationType}", activityName,
            typeof(T).Name);
        return activitySource.StartActivity(activityName, kind, propagationContext.ActivityContext, tags);
    }
}
