using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Time;

namespace Booking.Shared.Services;

public interface IBookingCheckoutSessionHelperService
{
    DateTimeOffset GetBookingPaymentExpiry(Database.Entities.Booking booking);
}

public class BookingCheckoutSessionHelperService : IBookingCheckoutSessionHelperService
{
    public DateTimeOffset GetBookingPaymentExpiry(Database.Entities.Booking booking)
    {
        if (string.IsNullOrWhiteSpace(booking.PaymentMethod))
        {
            return DateTimeOffset.MinValue;
        }

        var allowedTime = booking.ProductVersions.Count != 0
            ? booking.PaymentMethod switch
            {
                PaymentMethodConstants.Card => booking.ProductVersions.Select(item => item.MaxAllowedResourcesLockTimePaidViaCard).Min(),
                PaymentMethodConstants.BankTransfer => booking.ProductVersions.Select(item => item.MaxAllowedResourcesLockTimePaidViaBankTransfer)
                    .Min(),
                _ => throw new ArgumentOutOfRangeException()
            }
            : booking.PaymentMethod switch
            {
                PaymentMethodConstants.Card => Constants.DefaultMaxAllowedResourcesLockTimePaidViaCard,
                PaymentMethodConstants.BankTransfer => Constants.DefaultMaxAllowedResourcesLockTimePaidViaBankTransfer,
                _ => throw new ArgumentOutOfRangeException()
            };

        return booking.CreatedAt.TrimAllAfterSeconds().AddMinutes(allowedTime);
    }
}
