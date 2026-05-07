using Api.Shared.Services.Models;
using Booking.Api.GraphQL.Payment;

namespace Booking.Api.UnitTests.GraphQL.Payment.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class PaymentStatusesShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_All_Payment_Statuses(RootQuery sut)
    {
        var result = sut.PaymentStatuses().ToList();

        result.Count.ShouldBe(6);
        result.ShouldContain(item => item.Type == PaymentStatus.Pending && item.Name == PaymentStatus.Pending.ToPaymentStatusName());
        result.ShouldContain(item => item.Type == PaymentStatus.Rejected && item.Name == PaymentStatus.Rejected.ToPaymentStatusName());
        result.ShouldContain(item => item.Type == PaymentStatus.Confirmed && item.Name == PaymentStatus.Confirmed.ToPaymentStatusName());
        result.ShouldContain(item => item.Type == PaymentStatus.Expired && item.Name == PaymentStatus.Expired.ToPaymentStatusName());
        result.ShouldContain(item =>
            item.Type == PaymentStatus.RecordNeverCreated && item.Name == PaymentStatus.RecordNeverCreated.ToPaymentStatusName());
        result.ShouldContain(item =>
            item.Type == PaymentStatus.NoPaymentRequired && item.Name == PaymentStatus.NoPaymentRequired.ToPaymentStatusName());
    }
}
