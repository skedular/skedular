using System.Diagnostics;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace Enterprise.Shared.Telemetry;

public interface IActivityAccessor
{
    void AddEvent(string name, string tagPrefix, IDictionary<string, string> tags);
    void RecordException(Exception exception);
    IActivitySource GetActivitySource(string activitySourceName);
}

/// <summary>
///     Facade for working with the current activity and activity sources.
/// </summary>
public class ActivityAccessor : IActivityAccessor
{
    private static readonly ActivitySourceFacade s_noopActivitySourceName = new("noop");
    private readonly IActivityGetter _activityGetter;
    private readonly IDictionary<string, IActivitySource> _activitySources;
    private readonly ILogger<ActivityAccessor> _logger;

    public ActivityAccessor(
        IActivityGetter activityGetter,
        IEnumerable<IActivitySource> activitySources,
        ILogger<ActivityAccessor> logger)
    {
        _activityGetter = activityGetter;
        _logger = logger;
        _activitySources = activitySources.ToDictionary(source => source.Name);
    }

    public void RecordException(Exception exception)
    {
        var activity = _activityGetter.GetCurrent();

        if (activity == null)
        {
            return;
        }

        activity.SetStatus(ActivityStatusCode.Error, exception.Message);
        activity.RecordException(exception);
    }

    /// <summary>
    /// </summary>
    /// <param name="name">Required</param>
    /// <param name="tagPrefix">Required</param>
    /// <param name="tags"></param>
    public void AddEvent(string name, string tagPrefix, IDictionary<string, string> tags)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(tagPrefix);
        ArgumentNullException.ThrowIfNull(tags);

        var activity = _activityGetter.GetCurrent();

        if (activity is null)
        {
            return;
        }

        var eventTags = new ActivityTagsCollection();

        foreach (var (key, value) in tags)
        {
            eventTags.Add($"{tagPrefix}.{key}", value);
        }

        var activityEvent = new ActivityEvent(name, DateTimeOffset.Now, eventTags);

        activity.AddEvent(activityEvent);
    }

    public IActivitySource GetActivitySource(string activitySourceName)
    {
        if (_activitySources.TryGetValue(activitySourceName, out var source))
        {
            return source;
        }

        _logger.LogWarning(
            "Could not find activity source {ActivitySourceName}. Returning NO-OP activity source instead",
            activitySourceName);

        return s_noopActivitySourceName;
    }

    public void AddEvent(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var activity = _activityGetter.GetCurrent();

        if (activity is null)
        {
            return;
        }

        var activityEvent = new ActivityEvent(name);
        activity.AddEvent(activityEvent);
    }
}
