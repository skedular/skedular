using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscription = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;

namespace Booking.Shared.UnitTests.Repositories;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class FilterByPaymentStatusesShould
{
    private static IQueryable<MarketplaceBookingSubscription> BuildQueryable(
        IEnumerable<string> paymentStatuses) =>
        paymentStatuses
            .Select(status => new MarketplaceBookingSubscription
            {
                Status = MarketplaceBookingSubscriptionStatusConstants.Active,
                MarketplaceBooking = new MarketplaceBooking
                {
                    PaymentStatus = status,
                },
            })
            .AsQueryable();

    private static MarketplaceBookingSubscriptionSearchCriteria CriteriaWith(
        IReadOnlyList<PaymentStatus> paymentStatuses) =>
        new(
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            [],
            [],
            [],
            paymentStatuses);

    [Fact]
    public void Return_All_When_PaymentStatuses_Is_Empty()
    {
        var queryable = BuildQueryable([
            PaymentStatusConstants.Pending,
            PaymentStatusConstants.Confirmed,
            PaymentStatusConstants.Rejected,
        ]);

        var result = queryable.AddSearchCriteria(CriteriaWith([]), null).ToList();

        result.Count.ShouldBe(3);
    }

    [Fact]
    public void Return_Only_Matching_When_Single_PaymentStatus_Provided()
    {
        var queryable = BuildQueryable([
            PaymentStatusConstants.Pending,
            PaymentStatusConstants.Confirmed,
            PaymentStatusConstants.Pending,
        ]);

        var result = queryable
            .AddSearchCriteria(CriteriaWith([PaymentStatus.Pending]), null)
            .ToList();

        result.Count.ShouldBe(2);
        result.ShouldAllBe(item => item.MarketplaceBooking.PaymentStatus == PaymentStatusConstants.Pending);
    }

    [Fact]
    public void Return_Union_When_Multiple_PaymentStatuses_Provided()
    {
        var queryable = BuildQueryable([
            PaymentStatusConstants.Pending,
            PaymentStatusConstants.Confirmed,
            PaymentStatusConstants.Rejected,
            PaymentStatusConstants.Expired,
        ]);

        var result = queryable
            .AddSearchCriteria(CriteriaWith([PaymentStatus.Pending, PaymentStatus.Rejected]), null)
            .ToList();

        result.Count.ShouldBe(2);
        result.ShouldContain(item => item.MarketplaceBooking.PaymentStatus == PaymentStatusConstants.Pending);
        result.ShouldContain(item => item.MarketplaceBooking.PaymentStatus == PaymentStatusConstants.Rejected);
    }
}
