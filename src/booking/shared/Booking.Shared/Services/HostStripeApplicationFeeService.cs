using Api.Shared.Services.Models;
using Enterprise.Shared;
using Stripe.Checkout;

namespace Booking.Shared.Services;

public interface IHostStripeApplicationFeeService
{
    long? GetAmountInMinorUnits(string organizationType, decimal? commissionAmount);

    SessionPaymentIntentDataOptions? CreateDestinationCharge(
        string organizationType,
        string stripeConnectAccountId,
        decimal? commissionAmount);
}

public class HostStripeApplicationFeeService : IHostStripeApplicationFeeService
{
    public long? GetAmountInMinorUnits(string organizationType, decimal? commissionAmount)
    {
        if (organizationType != OrganizationTypeConstants.Host || commissionAmount is null or <= 0m)
        {
            return null;
        }

        return decimal.ToInt64((commissionAmount.Value * 100m).RoundedDecimal());
    }

    public SessionPaymentIntentDataOptions? CreateDestinationCharge(
        string organizationType,
        string stripeConnectAccountId,
        decimal? commissionAmount)
    {
        var applicationFeeAmount = GetAmountInMinorUnits(organizationType, commissionAmount);
        if (!applicationFeeAmount.HasValue)
        {
            return null;
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(stripeConnectAccountId);
        return new SessionPaymentIntentDataOptions
        {
            ApplicationFeeAmount = applicationFeeAmount.Value,
            TransferData = new SessionPaymentIntentDataTransferDataOptions { Destination = stripeConnectAccountId }
        };
    }
}
