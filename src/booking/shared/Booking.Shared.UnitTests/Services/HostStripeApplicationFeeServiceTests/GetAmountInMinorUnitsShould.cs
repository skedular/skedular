using Api.Shared.Services.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.HostStripeApplicationFeeServiceTests;

public class GetAmountInMinorUnitsShould
{
    [Theory]
    [InlineData(5.00, 500)]
    [InlineData(13.78, 1378)]
    public void ReturnHostCommissionInMinorUnits(decimal commissionAmount, long expectedAmount)
    {
        var sut = new HostStripeApplicationFeeService();

        sut.GetAmountInMinorUnits(OrganizationTypeConstants.Host, commissionAmount).ShouldBe(expectedAmount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void IgnoreNonPositiveCommission(decimal commissionAmount)
    {
        var sut = new HostStripeApplicationFeeService();

        sut.GetAmountInMinorUnits(OrganizationTypeConstants.Host, commissionAmount).ShouldBeNull();
    }

    [Fact]
    public void IgnoreMarketplaceOrganization()
    {
        var sut = new HostStripeApplicationFeeService();

        sut.GetAmountInMinorUnits(OrganizationTypeConstants.Marketplace, 5m).ShouldBeNull();
    }
}
