using Api.Shared.Services.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.HostStripeApplicationFeeServiceTests;

public class CreateDestinationChargeShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void RouteHostProceedsAndRetainCommission(
        HostStripeApplicationFeeService sut,
        string stripeConnectAccountId,
        decimal commissionAmount)
    {
        commissionAmount = Math.Abs(decimal.Round(commissionAmount, 2)) + 0.01m;

        var result = sut.CreateDestinationCharge(
            OrganizationTypeConstants.Host,
            stripeConnectAccountId,
            commissionAmount);

        result.ShouldNotBeNull();
        result.ApplicationFeeAmount.ShouldBe(sut.GetAmountInMinorUnits(OrganizationTypeConstants.Host, commissionAmount));
        result.TransferData.ShouldNotBeNull();
        result.TransferData.Destination.ShouldBe(stripeConnectAccountId);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void IgnoreNonHostOrganizations(
        HostStripeApplicationFeeService sut,
        string stripeConnectAccountId,
        decimal commissionAmount) =>
        sut.CreateDestinationCharge(
                OrganizationTypeConstants.Marketplace,
                stripeConnectAccountId,
                commissionAmount)
            .ShouldBeNull();
}
