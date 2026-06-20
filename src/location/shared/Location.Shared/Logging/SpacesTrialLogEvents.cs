using Microsoft.Extensions.Logging;

namespace Location.Shared.Logging;

public static class SpacesTrialLogEvents
{
    public static readonly EventId AccessDecisionAllowed = new(1500, nameof(AccessDecisionAllowed));
    public static readonly EventId AccessDecisionDenied = new(1501, nameof(AccessDecisionDenied));
    public static readonly EventId ProjectionUpdated = new(1502, nameof(ProjectionUpdated));
    public static readonly EventId ProjectionFailed = new(1503, nameof(ProjectionFailed));
    public static readonly EventId TeamsBypass = new(1504, nameof(TeamsBypass));
}
