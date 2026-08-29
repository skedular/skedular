using Api.Shared.Services.Models;
using Booking.Shared.Models.Entitlements;

namespace Booking.Shared.UnitTests.Services.Entitlements;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EntitlementStateMappingShould
{
    [Fact]
    public void RoundTripPersistedStateValues()
    {
        ProductPricingFulfillmentType.Entitlement.ToPersistedValue().ShouldBe("ENTITLEMENT");
        "ENTITLEMENT".FromPersistedValue().ShouldBe(ProductPricingFulfillmentType.Entitlement);
        EntitlementStatus.Active.ToPersistedValue().ShouldBe("ACTIVE");
        EntitlementLifecycleStateExtensions.EntitlementStatusFromPersistedValue("ACTIVE").ShouldBe(EntitlementStatus.Active);
        EntitlementStatus.Cancelled.ToPersistedValue().ShouldBe("CANCELLED");
        EntitlementLifecycleStateExtensions.EntitlementStatusFromPersistedValue("CANCELLED").ShouldBe(EntitlementStatus.Cancelled);
        EntitlementRenewalStatus.Confirmed.ToPersistedValue().ShouldBe("CONFIRMED");
        EntitlementLifecycleStateExtensions.RenewalStatusFromPersistedValue("CONFIRMED").ShouldBe(EntitlementRenewalStatus.Confirmed);
        EntitlementRefundStatus.ManualSettlementRequired.ToPersistedValue().ShouldBe("MANUAL_SETTLEMENT_REQUIRED");
        EntitlementLifecycleStateExtensions.RefundStatusFromPersistedValue("MANUAL_SETTLEMENT_REQUIRED")
            .ShouldBe(EntitlementRefundStatus.ManualSettlementRequired);
    }
}
