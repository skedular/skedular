using Booking.Shared.Models.Entitlements;

namespace Booking.Shared.UnitTests.Models.Entitlements;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EntitlementStatusShould
{
    [Fact]
    public void PreserveStablePersistedValues()
    {
        Assert.Equal(1, (int)EntitlementStatus.Active);
        Assert.Equal(2, (int)EntitlementStatus.Expired);
        Assert.Equal(0, (int)CreditLedgerTransactionType.Granted);
        Assert.Equal(1, (int)CreditLedgerTransactionType.Consumed);
        Assert.Equal(4, (int)CreditLedgerTransactionType.Expired);
    }
}
