using Api.Shared.Services.Models;
using Booking.Api.GraphQL.Payment;

namespace Booking.Api.UnitTests.GraphQL.Payment.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class PaymentMethodTypesShould
{
    [Fact]
    public void Return_All_Payment_Method_Types()
    {
        var sut = new RootQuery();

        var result = sut.PaymentMethodTypes().ToList();

        result.Count.ShouldBe(2);
        result.ShouldContain(item =>
            item.Type == PaymentMethod.Card &&
            item.Name == PaymentMethodConstants.Card.ToPaymentMethodName());
        result.ShouldContain(item =>
            item.Type == PaymentMethod.BankTransfer &&
            item.Name == PaymentMethodConstants.BankTransfer.ToPaymentMethodName());
    }
}
