using Api.Shared.Services.Models;

namespace Booking.Shared.Services.Entitlements;

internal static class MarketplaceEntitlementInstallmentAllocation
{
    public static int GetAmount(int total, int count, int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);
        index = Math.Clamp(index, 0, count - 1);
        return total / count + (index < total % count ? 1 : 0);
    }

    public static int GetIndex(
        DateTimeOffset serviceStartAt,
        DateTimeOffset billingPeriodStartAt,
        OrganizationBillingCycle billingCycle,
        int installmentCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(installmentCount);
        if (billingPeriodStartAt <= serviceStartAt)
        {
            return 0;
        }

        var index = 0;
        var cursor = serviceStartAt;
        while (index < installmentCount - 1 && cursor < billingPeriodStartAt)
        {
            cursor = billingCycle switch
            {
                OrganizationBillingCycle.Weekly => cursor.AddDays(7),
                OrganizationBillingCycle.Fortnightly => cursor.AddDays(14),
                OrganizationBillingCycle.Monthly => cursor.AddMonths(1),
                _ => throw new ArgumentOutOfRangeException(nameof(billingCycle)),
            };
            index++;
        }

        return index;
    }
}
