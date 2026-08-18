using Booking.Shared.Database.Entities;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Services.Entitlements;

namespace Booking.Shared.UnitTests.Services.Entitlements.CreditLedgerServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetAvailableCreditsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void IncludeGrantsReleasesAndAdjustmentsAndExcludeConsumedTerminalEntries(CreditLedgerService sut)
    {
        var entitlement = new Entitlement
        {
            GrantedQuantity = 10,
            LedgerEntries =
            [
                new CreditLedgerEntry
                {
                    Quantity = 2,
                    TransactionType = CreditLedgerTransactionType.Consumed.ToPersistedValue(),
                },
                new CreditLedgerEntry
                {
                    Quantity = 1,
                    TransactionType = CreditLedgerTransactionType.Released.ToPersistedValue(),
                },
                new CreditLedgerEntry
                {
                    Quantity = 2,
                    TransactionType = CreditLedgerTransactionType.Adjusted.ToPersistedValue(),
                },
                new CreditLedgerEntry
                {
                    Quantity = 1,
                    TransactionType = CreditLedgerTransactionType.Forfeited.ToPersistedValue(),
                },
            ],
        };

        Assert.Equal(10, sut.GetAvailableCredits(entitlement));
    }
}
