using Api.Shared.Services.Models;
using Booking.Shared.Models;

namespace Booking.Shared.Services;

public class MarketplaceRefundPolicyService
{
    public MarketplaceRefundQuote GetQuote(
        ProductPricing pricing,
        DateTimeOffset referenceTime,
        DateTimeOffset requestedAt)
    {
        if (pricing.CancellationPolicyType == ProductPricingCancellationPolicyType.NoCancellation)
        {
            return new MarketplaceRefundQuote(false, false, 0, null);
        }

        if (pricing.CancellationPolicyType == ProductPricingCancellationPolicyType.FullRefundBeforeCutoff &&
            pricing.CancellationRefundRules.Count == 0)
        {
            return requestedAt <= referenceTime
                ? new MarketplaceRefundQuote(true, true, 100, 0)
                : new MarketplaceRefundQuote(false, false, 0, null);
        }

        var applicableRule = pricing.CancellationRefundRules
            .OrderByDescending(item => item.MinutesBefore)
            .FirstOrDefault(item => requestedAt <= referenceTime.AddMinutes(-item.MinutesBefore));

        if (applicableRule is null)
        {
            return new MarketplaceRefundQuote(false, false, 0, null);
        }

        var refundPercentage = Math.Clamp(applicableRule.RefundPercentage, 0, 100);
        return new MarketplaceRefundQuote(true, refundPercentage > 0, refundPercentage, applicableRule.MinutesBefore);
    }
}
