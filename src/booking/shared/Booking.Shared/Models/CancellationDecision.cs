using Api.Shared.Services.Models;

namespace Booking.Shared.Models;

public enum CancellationErrorCode
{
    PolicyRestriction,
    OverrideReasonRequired,
    InsufficientManagementPermission,
    InvalidTerminalState,
}

public enum CancellationActorCategory
{
    Customer,
    Owner,
    Administrator,
}

public sealed record CancellationActor(
    CancellationActorCategory Category,
    string CustomerId,
    string? OrganizationId = null);

public sealed record CancellationDecision(
    CancellationActor Actor,
    bool CanOverridePolicy,
    string? OverrideReason)
{
    public bool HasOverrideReason => !string.IsNullOrWhiteSpace(OverrideReason);
}

public sealed record CancellationRequest(
    string TargetId,
    MarketplaceBookingSubscriptionCancellationMode? SubscriptionCancellationMode,
    CancellationDecision Decision);
