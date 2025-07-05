using Api.Shared.Services.Models;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Payment;

[QueryType]
public class RootQuery
{
    [UseResolverScope]
    public IEnumerable<BookingPaymentMethodTypeDetails> BookingPaymentMethodTypes() =>
    [
        new() { Type = BookingPaymentMethod.Card, Name = BookingPaymentMethodConstants.Card.ToBookingPaymentMethodName() },
        new() { Type = BookingPaymentMethod.BankAccount, Name = BookingPaymentMethodConstants.BankAccount.ToBookingPaymentMethodName() }
    ];
}
