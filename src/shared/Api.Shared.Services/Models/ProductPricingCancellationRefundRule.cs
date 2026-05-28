using HotChocolate.Types.Composite;

namespace Api.Shared.Services.Models;

[Shareable]
public record ProductPricingCancellationRefundRule(int MinutesBefore, int RefundPercentage)
{
    public static ProductPricingCancellationRefundRule Empty => new(int.MinValue, int.MinValue);
}
