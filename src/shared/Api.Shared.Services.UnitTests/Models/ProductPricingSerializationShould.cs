using System.Text.Json;
using Api.Shared.Services.Models;

namespace Api.Shared.Services.UnitTests.Models;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ProductPricingSerializationShould
{
    [Fact]
    public void Preserve_Duration_Display_Units_And_Canonical_Minutes()
    {
        var pricing = ProductPricing.Empty("pricing") with
        {
            MinDurationMinutes = 5,
            MinDurationDisplayUnit = DurationDisplayUnit.Minutes,
            CancellationRefundRules = [new ProductPricingCancellationRefundRule(5, 100, DurationDisplayUnit.Hours)],
        };

        var json = JsonSerializer.Serialize(pricing);
        var restored = JsonSerializer.Deserialize<ProductPricing>(json);

        restored!.MinDurationMinutes.ShouldBe(5);
        restored.MinDurationDisplayUnit.ShouldBe(DurationDisplayUnit.Minutes);
        restored.CancellationRefundRules[0].MinutesBefore.ShouldBe(5);
        restored.CancellationRefundRules[0].DisplayUnit.ShouldBe(DurationDisplayUnit.Hours);
    }

    [Fact]
    public void Treat_Missing_Duration_Display_Unit_As_Null()
    {
        var pricing = JsonSerializer.Deserialize<ProductPricing>(JsonSerializer.Serialize(ProductPricing.Empty("pricing")));

        pricing!.MinDurationDisplayUnit.ShouldBeNull();
        pricing.MaxDurationDisplayUnit.ShouldBeNull();
    }
}
