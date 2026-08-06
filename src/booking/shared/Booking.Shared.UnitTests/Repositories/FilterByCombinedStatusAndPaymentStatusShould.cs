using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscription = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;

namespace Booking.Shared.UnitTests.Repositories;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class FilterByCombinedStatusAndPaymentStatusShould
{
    private static IQueryable<MarketplaceBookingSubscription> BuildQueryable() =>
        new List<MarketplaceBookingSubscription>
        {
            new()
            {
                Status = MarketplaceBookingSubscriptionStatusConstants.Active,
                MarketplaceBooking = new MarketplaceBooking
                {
                    PaymentStatus = PaymentStatusConstants.Pending,
                },
            },
            new()
            {
                Status = MarketplaceBookingSubscriptionStatusConstants.Active,
                MarketplaceBooking = new MarketplaceBooking
                {
                    PaymentStatus = PaymentStatusConstants.Confirmed,
                },
            },
            new()
            {
                Status = MarketplaceBookingSubscriptionStatusConstants.Cancelled,
                MarketplaceBooking = new MarketplaceBooking
                {
                    PaymentStatus = PaymentStatusConstants.Pending,
                },
            },
            new()
            {
                Status = MarketplaceBookingSubscriptionStatusConstants.Cancelled,
                MarketplaceBooking = new MarketplaceBooking
                {
                    PaymentStatus = PaymentStatusConstants.Confirmed,
                },
            },
        }.AsQueryable();

    private static MarketplaceBookingSubscriptionSearchCriteria CriteriaWith(
        IReadOnlyList<MarketplaceBookingSubscriptionStatus> statuses,
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
            statuses,
            paymentStatuses);

    [Fact]
    public void Return_Intersection_When_Both_Filters_Set()
    {
        var result = BuildQueryable()
            .AddSearchCriteria(CriteriaWith(
                    [MarketplaceBookingSubscriptionStatus.Active],
                    [PaymentStatus.Pending]),
                null)
            .ToList();

        result.Count.ShouldBe(1);
        result[0].Status.ShouldBe(MarketplaceBookingSubscriptionStatusConstants.Active);
        result[0].MarketplaceBooking.PaymentStatus.ShouldBe(PaymentStatusConstants.Pending);
    }

    [Fact]
    public void Return_All_Matching_Payment_Status_When_Status_Filter_Cleared()
    {
        var result = BuildQueryable()
            .AddSearchCriteria(CriteriaWith([], [PaymentStatus.Pending]), null)
            .ToList();

        result.Count.ShouldBe(2);
        result.ShouldAllBe(item => item.MarketplaceBooking.PaymentStatus == PaymentStatusConstants.Pending);
    }

    [Fact]
    public void Return_All_Matching_Status_When_PaymentStatus_Filter_Cleared()
    {
        var result = BuildQueryable()
            .AddSearchCriteria(CriteriaWith([MarketplaceBookingSubscriptionStatus.Cancelled], []), null)
            .ToList();

        result.Count.ShouldBe(2);
        result.ShouldAllBe(item => item.Status == MarketplaceBookingSubscriptionStatusConstants.Cancelled);
    }

    [Fact]
    public void Return_All_When_Both_Filters_Empty()
    {
        var result = BuildQueryable()
            .AddSearchCriteria(CriteriaWith([], []), null)
            .ToList();

        result.Count.ShouldBe(4);
    }
}
