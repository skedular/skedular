using System.Diagnostics;

namespace Enterprise.Shared.Telemetry;

public interface IActivitySource
{
    /// <summary>
    ///     Name of the activity source
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Start an activity. Optionally provide a parent context
    /// </summary>
    /// <param name="name">Activity name</param>
    /// <param name="kind">
    ///     Describes the relationship between the activity, its parents and its children in a trace.
    ///     https://learn.microsoft.com/en-us/dotnet/api/System.Diagnostics.ActivityKind?view=net-7.0
    /// </param>
    /// <param name="parentContext">Parent context if available</param>
    /// <param name="tags">Associated tags</param>
    /// <returns></returns>
    Activity? StartActivity(
        string name,
        ActivityKind kind = ActivityKind.Internal,
        ActivityContext parentContext = default,
        IEnumerable<KeyValuePair<string, object?>>? tags = default);
}

public class ActivitySourceFacade(string name, string? version = null) : IActivitySource
{
    private readonly ActivitySource _activitySource = new(name, version);

    public string Name { get; } = name;

    public Activity? StartActivity(
        string name,
        ActivityKind kind,
        ActivityContext parentContext = default,
        IEnumerable<KeyValuePair<string, object?>>? tags = default) =>
        _activitySource.StartActivity(
            name,
            kind,
            parentContext,
            tags);
}
