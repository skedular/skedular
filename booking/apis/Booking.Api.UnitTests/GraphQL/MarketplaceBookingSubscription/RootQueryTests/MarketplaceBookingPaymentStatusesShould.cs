using Api.Shared.Services.Models;
using Booking.Api.GraphQL.MarketplaceBookingSubscription;

namespace Booking.Api.UnitTests.GraphQL.MarketplaceBookingSubscription.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceBookingPaymentStatusesShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_All_Operator_Relevant_Payment_Status_Options(RootQuery sut)
    {
        var result = sut.MarketplaceBookingPaymentStatuses().ToList();

        result.Count.ShouldBe(6);
        result.ShouldContain(item => item.Type == PaymentStatus.NotSet && !string.IsNullOrWhiteSpace(item.Name));
        result.ShouldContain(item => item.Type == PaymentStatus.Pending && !string.IsNullOrWhiteSpace(item.Name));
        result.ShouldContain(item => item.Type == PaymentStatus.Rejected && !string.IsNullOrWhiteSpace(item.Name));
        result.ShouldContain(item => item.Type == PaymentStatus.Confirmed && !string.IsNullOrWhiteSpace(item.Name));
        result.ShouldContain(item => item.Type == PaymentStatus.Expired && !string.IsNullOrWhiteSpace(item.Name));
        result.ShouldContain(item => item.Type == PaymentStatus.NoPaymentRequired && !string.IsNullOrWhiteSpace(item.Name));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Payment_Status_Options_With_Correct_Names(RootQuery sut)
    {
        var result = sut.MarketplaceBookingPaymentStatuses().ToList();

        result.ShouldContain(item =>
            item.Type == PaymentStatus.Pending &&
            item.Name == PaymentStatus.Pending.ToMarketplaceBookingPaymentStatusName());
        result.ShouldContain(item =>
            item.Type == PaymentStatus.NoPaymentRequired &&
            item.Name == PaymentStatus.NoPaymentRequired.ToMarketplaceBookingPaymentStatusName());
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Not_Include_RecordNeverCreated_In_Operator_Facing_Options(RootQuery sut)
    {
        var result = sut.MarketplaceBookingPaymentStatuses().ToList();

        result.ShouldNotContain(item => item.Type == PaymentStatus.RecordNeverCreated);
    }
}
