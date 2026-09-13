using Api.Shared.Services.Models;
using Booking.Shared.Services.Entitlements;

namespace Booking.Shared.UnitTests.Services.Entitlements;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public sealed class MarketplaceEntitlementInstallmentAllocationShould
{
    [Theory]
    [InlineData(7, 6, 0, 2)]
    [InlineData(7, 6, 1, 1)]
    [InlineData(7, 6, 5, 1)]
    public void Allocate_remainder_once(int total, int count, int index, int expected) =>
        MarketplaceEntitlementInstallmentAllocation.GetAmount(total, count, index).ShouldBe(expected);

    [Fact]
    public void Return_the_same_index_for_repeated_or_out_of_order_periods()
    {
        var start = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var secondPeriod = start.AddMonths(1);
        var laterPeriod = start.AddMonths(2);

        MarketplaceEntitlementInstallmentAllocation.GetIndex(
            start, laterPeriod, OrganizationBillingCycle.Monthly, 6).ShouldBe(2);
        MarketplaceEntitlementInstallmentAllocation.GetIndex(
            start, secondPeriod, OrganizationBillingCycle.Monthly, 6).ShouldBe(1);
        MarketplaceEntitlementInstallmentAllocation.GetIndex(
            start, secondPeriod, OrganizationBillingCycle.Monthly, 6).ShouldBe(1);
    }

    [Theory]
    [InlineData(7, 6)]
    [InlineData(100, 3)]
    public void Preserve_the_total_credit_quantity(int total, int count) =>
        Enumerable.Range(0, count)
            .Sum(index => MarketplaceEntitlementInstallmentAllocation.GetAmount(total, count, index))
            .ShouldBe(total);

    [Theory]
    [InlineData(1, 0, 0)]
    public void Reject_invalid_installment_allocation(int total, int count, int index) =>
        Should.Throw<ArgumentOutOfRangeException>(() =>
            MarketplaceEntitlementInstallmentAllocation.GetAmount(total, count, index));
}
