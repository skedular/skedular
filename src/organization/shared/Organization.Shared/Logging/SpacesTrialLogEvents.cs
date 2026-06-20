using Microsoft.Extensions.Logging;

namespace Organization.Shared.Logging;

public static class SpacesTrialLogEvents
{
    public static readonly EventId InitializationStarted = new(1500, nameof(InitializationStarted));
    public static readonly EventId InitializationCompleted = new(1501, nameof(InitializationCompleted));
    public static readonly EventId InitializationAlreadyPresent = new(1502, nameof(InitializationAlreadyPresent));
    public static readonly EventId CreationDateFallbackApplied = new(1503, nameof(CreationDateFallbackApplied));
    public static readonly EventId StatusEvaluated = new(1504, nameof(StatusEvaluated));
    public static readonly EventId WarningObserved = new(1505, nameof(WarningObserved));
    public static readonly EventId ExpiryObserved = new(1506, nameof(ExpiryObserved));
    public static readonly EventId UpgradeRequested = new(1507, nameof(UpgradeRequested));
    public static readonly EventId ComplimentaryBridgeStarted = new(1508, nameof(ComplimentaryBridgeStarted));
    public static readonly EventId BillingBoundaryScheduled = new(1509, nameof(BillingBoundaryScheduled));
    public static readonly EventId BillingTransitionCompleted = new(1510, nameof(BillingTransitionCompleted));
    public static readonly EventId BillingTransitionFailed = new(1511, nameof(BillingTransitionFailed));
    public static readonly EventId TeamsBypass = new(1512, nameof(TeamsBypass));
}
