using System.Diagnostics;

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
public class ActivityGetter : IActivityGetter
{
    public Activity? GetCurrent() => Activity.Current;
}
