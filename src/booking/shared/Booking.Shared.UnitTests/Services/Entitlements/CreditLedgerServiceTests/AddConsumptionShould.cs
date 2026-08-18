using Booking.Shared.Database.Entities;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Services.Entitlements;
using Enterprise.Shared.Random;

namespace Booking.Shared.UnitTests.Services.Entitlements.CreditLedgerServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddConsumptionShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void ReturnTheExistingEntryForAnIdempotencyKey([Frozen] IRandomHelper randomHelper, CreditLedgerService sut)
    {
        var existing = new CreditLedgerEntry
        {
            Id = "existing",
            ReferenceKey = "request-1",
            Quantity = 1,
            TransactionType = CreditLedgerTransactionType.Consumed.ToPersistedValue(),
        };
        var entitlement = new Entitlement
        {
            Id = "entitlement-1",
            GrantedQuantity = 1,
            LedgerEntries = [existing],
        };

        var result = sut.AddConsumption(entitlement, "booking-1", "request-1", TimeProvider.System.GetUtcNow());

        Assert.Same(existing, result);
        A.CallTo(() => randomHelper.Generate()).MustNotHaveHappened();
    }
}
