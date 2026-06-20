using Api.Shared.Services.Models;
using Enterprise.Shared;
using Microsoft.Extensions.Logging;

namespace Booking.Shared.Services;

public record HostCommission(decimal RatePercentage, decimal Amount, decimal HostPayoutAmount);

public interface IHostCommissionService
{
    HostCommission? Calculate(string organizationType, decimal commissionRatePercentage, decimal bookingTotal);
}

public class HostCommissionService(ILogger<HostCommissionService> logger) : IHostCommissionService
{
    public HostCommission? Calculate(string organizationType, decimal commissionRatePercentage, decimal bookingTotal)
    {
        if (organizationType != OrganizationTypeConstants.Host)
        {
            return null;
        }

        var rate = Math.Clamp(commissionRatePercentage, 0m, 100m).RoundedDecimal();
        var amount = (bookingTotal * rate / 100m).RoundedDecimal();
        var result = new HostCommission(rate, amount, (bookingTotal - amount).RoundedDecimal());
        logger.LogInformation(
            "Host commission calculated. RatePercentage: {RatePercentage}, BookingTotal: {BookingTotal}, CommissionAmount: {CommissionAmount}, HostPayoutAmount: {HostPayoutAmount}",
            result.RatePercentage,
            bookingTotal,
            result.Amount,
            result.HostPayoutAmount);
        return result;
    }
}
