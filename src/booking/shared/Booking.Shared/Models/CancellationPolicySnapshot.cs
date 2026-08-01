namespace Booking.Shared.Models;

public sealed record CancellationPolicySnapshot(
    string PolicyType,
    IReadOnlyList<CancellationRefundRuleSnapshot> Rules,
    DateTimeOffset CapturedAt,
    string ProductPriceId);

public sealed record CancellationRefundRuleSnapshot(int MinutesBefore, int RefundPercentage);
