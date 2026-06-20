using Microsoft.Extensions.Logging;

namespace Booking.Shared.Logging;

public static class SpacesTrialLogEvents
{
    public static readonly EventId AccessDecisionAllowed = new(1500, nameof(AccessDecisionAllowed));
    public static readonly EventId AccessDecisionDenied = new(1501, nameof(AccessDecisionDenied));
    public static readonly EventId UsageQueryBypassed = new(1502, nameof(UsageQueryBypassed));
    public static readonly EventId RecurringCommitmentSuppressed = new(1503, nameof(RecurringCommitmentSuppressed));
    public static readonly EventId PublicAvailabilityEvaluated = new(1504, nameof(PublicAvailabilityEvaluated));
    public static readonly EventId ProjectionUpdated = new(1505, nameof(ProjectionUpdated));
    public static readonly EventId ProjectionFailed = new(1506, nameof(ProjectionFailed));
    public static readonly EventId TeamsBypass = new(1507, nameof(TeamsBypass));
}
