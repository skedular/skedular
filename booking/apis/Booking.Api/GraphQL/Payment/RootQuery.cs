using Api.Shared.Services.Models;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Payment;

[QueryType]
public class RootQuery
{
    [UseResolverScope]
    public IEnumerable<PaymentMethodTypeDetails> PaymentMethodTypes() =>
    [
        new() { Type = PaymentMethod.Card, Name = PaymentMethodConstants.Card.ToPaymentMethodName() },
        new() { Type = PaymentMethod.BankTransfer, Name = PaymentMethodConstants.BankTransfer.ToPaymentMethodName() }
    ];
}
