namespace Api.Shared.Services.Models;

public record ProductPricingCancellationRefundRule(int MinutesBefore, int RefundPercentage)
{
    public static ProductPricingCancellationRefundRule Empty => new(int.MinValue, int.MinValue);
}
