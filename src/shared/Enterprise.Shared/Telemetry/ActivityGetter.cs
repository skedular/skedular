using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Telemetry;

/// <summary>
///     An abstraction of the Activity.Current property
/// </summary>
public interface IActivityGetter
{
    Activity? GetCurrent();
}

/// <summary>
///     An abstraction of the Activity.Current
/// </summary>
public class ActivityGetter(ILogger<ActivityGetter> logger) : IActivityGetter
{
    public Activity? GetCurrent()
    {
        var current = Activity.Current;
        logger.LogDebug("Retrieved current activity. HasCurrentActivity={HasCurrentActivity}", current is not null);
        return current;
    }
}
