using Api.Shared.Services.Models;
using Booking.Shared.Services;
using Microsoft.Extensions.Logging;
using static Testing.Shared.Assertions.LogAssertions;

namespace Booking.Shared.UnitTests.Services.HostCommissionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CalculateShould
{
    [Theory]
    [InlineAutoFakeItEasyData(new Type[] { }, 100, 5, 5, 95)]
    [InlineAutoFakeItEasyData(new Type[] { }, 275.50, 5, 13.78, 261.72)]
    public void CalculateConfiguredCommissionForHost(
        decimal bookingTotal,
        decimal commissionRate,
        decimal expectedCommission,
        decimal expectedPayout,
        [Frozen]
        ILogger<HostCommissionService> logger,
        HostCommissionService sut)
    {
        var result = sut.Calculate(OrganizationTypeConstants.Host, commissionRate, bookingTotal);

        result.ShouldNotBeNull();
        result.Amount.ShouldBe(expectedCommission);
        result.HostPayoutAmount.ShouldBe(expectedPayout);
        ACallToLog(logger, LogLevel.Information).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void IgnoreNonHostOrganization(
        [Frozen]
        ILogger<HostCommissionService> logger,
        HostCommissionService sut,
        decimal commissionRate,
        decimal bookingTotal)
    {
        sut.Calculate(OrganizationTypeConstants.Marketplace, commissionRate, bookingTotal).ShouldBeNull();
        ACallToLog(logger, LogLevel.Information).MustNotHaveHappened();
    }
}
