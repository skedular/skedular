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

    [UseResolverScope]
    public IEnumerable<PaymentStatusDetails> PaymentStatuses() =>
    [
        new() { Type = PaymentStatus.Pending, Name = PaymentStatusConstants.Pending.ToPaymentStatusName() },
        new() { Type = PaymentStatus.Rejected, Name = PaymentStatusConstants.Rejected.ToPaymentStatusName() },
        new() { Type = PaymentStatus.Confirmed, Name = PaymentStatusConstants.Confirmed.ToPaymentStatusName() },
        new() { Type = PaymentStatus.Expired, Name = PaymentStatusConstants.Expired.ToPaymentStatusName() },
        new() { Type = PaymentStatus.RecordNeverCreated, Name = PaymentStatusConstants.RecordNeverCreated.ToPaymentStatusName() },
        new() { Type = PaymentStatus.NoPaymentRequired, Name = PaymentStatusConstants.NoPaymentRequired.ToPaymentStatusName() }
    ];
}
