using Enterprise.Shared;

namespace Booking.Shared.Models;

public record MarketplaceRefundQuote(
    bool CanCancel,
    bool IsRefundable,
    int RefundPercentage,
    int? AppliedRuleMinutesBefore)
{
    public decimal CalculateRefundAmount(decimal totalAmount) => (totalAmount * RefundPercentage / 100m).RoundedDecimal();
}
