namespace Booking.Shared.Models;

public sealed record MarketplaceRefundCalculationResult(
    decimal OriginalGrossAmount,
    decimal EligibleRefundAmount,
    decimal CancellationDeduction,
    decimal TaxAdjustment,
    decimal PreviouslyRefundedAmount,
    decimal FinalRefundableAmount,
    decimal NonRefundableAmount,
    string CalculationReason,
    CancellationPolicySnapshot PolicySnapshotUsed,
    DateTimeOffset CalculatedAt,
    DateTimeOffset CancellationTime,
    DateTimeOffset ReferenceTime,
    string TimezoneId);
