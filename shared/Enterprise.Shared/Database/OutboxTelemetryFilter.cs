using System.Diagnostics;
using TelemetryKeys = Enterprise.Shared.Outbox.Kafka.TelemetryKeys;

namespace Enterprise.Shared.Database;

public static class OutboxTelemetryFilter
{
    private static readonly HashSet<string> s_excludedActivitySourceNames =
    [
        TelemetryKeys.KafkaActivitySourceName,
        Outbox.Temporal.TelemetryKeys.TemporalActivitySourceName
    ];

    private static readonly HashSet<string> s_excludedActivityNames =
    [
        TelemetryKeys.KafkaEventPoll,
        TelemetryKeys.KafkaEventSave,
        TelemetryKeys.KafkaEventSend,
        Outbox.Temporal.TelemetryKeys.TemporalEventPoll,
        Outbox.Temporal.TelemetryKeys.TemporalEventSave,
        Outbox.Temporal.TelemetryKeys.TemporalEventSend
    ];

    public static bool ShouldTraceActivity(Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);

        return HasError(activity) || (!IsOutboxActivity(activity) && !HasOutboxParent(activity));
    }

    private static bool IsOutboxActivity(Activity activity) =>
        s_excludedActivitySourceNames.Contains(activity.Source.Name)
        || s_excludedActivityNames.Contains(activity.OperationName)
        || s_excludedActivityNames.Contains(activity.DisplayName);

    private static bool HasOutboxParent(Activity activity)
    {
        var parent = activity.Parent;
        while (parent is not null)
        {
            if (IsOutboxActivity(parent))
            {
                return true;
            }

            parent = parent.Parent;
        }

        return false;
    }

    private static bool HasError(Activity activity)
    {
        if (activity.Status == ActivityStatusCode.Error)
        {
            return true;
        }

        foreach (var tag in activity.TagObjects)
        {
            switch (tag.Key)
            {
                case "error.type" when tag.Value is not null:
                case "otel.status_code" when string.Equals(tag.Value?.ToString(), "ERROR", StringComparison.OrdinalIgnoreCase):
                    return true;
            }
        }

        return activity.Events.Any(activityEvent => string.Equals(activityEvent.Name, "exception", StringComparison.OrdinalIgnoreCase));
    }
}
