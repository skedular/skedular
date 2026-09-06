using System.Text.Json.Serialization;
using HotChocolate.Types.Composite;

namespace Api.Shared.Services.Models;

[Shareable]
public record ProductPricingCancellationRefundRule(
    int MinutesBefore,
    int RefundPercentage,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    DurationDisplayUnit? DisplayUnit = null)
{
    public static ProductPricingCancellationRefundRule Empty => new(int.MinValue, int.MinValue);
}
