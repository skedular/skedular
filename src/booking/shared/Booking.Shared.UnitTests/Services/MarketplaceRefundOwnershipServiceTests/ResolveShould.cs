using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundOwnershipServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ResolveShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Prefer_OneTime_Booking_Owner(MarketplaceRefundOwnershipService sut, string bookingId, string recurringBookingId,
        string subscriptionId)
    {
        var result = sut.Resolve(new MarketplaceBookingFailure
        {
            Id = "failure-1",
            BookingId = bookingId,
            RecurringBookingId = recurringBookingId,
            MarketplaceBookingSubscriptionId = subscriptionId,
        });

        result.Scope.ShouldBe(MarketplaceRefundOwnershipScope.OneTimeBooking);
        result.LocalEntityType.ShouldBe(MarketplaceRefundEntityTypeConstants.MarketplaceBooking);
        result.LocalEntityId.ShouldBe(bookingId);
        result.BookingId.ShouldBe(bookingId);
        result.RecurringBookingId.ShouldBe(recurringBookingId);
        result.MarketplaceBookingSubscriptionId.ShouldBe(subscriptionId);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Resolve_Subscription_Window_Owner(MarketplaceRefundOwnershipService sut, string subscriptionId, string recurringBookingId)
    {
        var result = sut.Resolve(new MarketplaceBookingFailure
        {
            Id = "failure-1",
            MarketplaceBookingSubscriptionId = subscriptionId,
            RecurringBookingId = recurringBookingId,
        });

        result.Scope.ShouldBe(MarketplaceRefundOwnershipScope.SubscriptionBillingWindow);
        result.LocalEntityType.ShouldBe(MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription);
        result.LocalEntityId.ShouldBe(subscriptionId);
        result.BookingId.ShouldBeNull();
        result.RecurringBookingId.ShouldBe(recurringBookingId);
        result.MarketplaceBookingSubscriptionId.ShouldBe(subscriptionId);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Resolve_Recurring_Window_Owner(MarketplaceRefundOwnershipService sut, string recurringBookingId)
    {
        var result = sut.Resolve(new MarketplaceBookingFailure
        {
            Id = "failure-1",
            RecurringBookingId = recurringBookingId,
        });

        result.Scope.ShouldBe(MarketplaceRefundOwnershipScope.RecurringBillingWindow);
        result.LocalEntityType.ShouldBe(MarketplaceRefundEntityTypeConstants.MarketplaceBooking);
        result.LocalEntityId.ShouldBe(recurringBookingId);
        result.RecurringBookingId.ShouldBe(recurringBookingId);
        result.MarketplaceBookingSubscriptionId.ShouldBeNull();
    }

    [Fact]
    public void Reject_Failure_With_No_Billable_Owner()
    {
        var exception = Should.Throw<InvalidOperationException>(() =>
            new MarketplaceRefundOwnershipService().Resolve(new MarketplaceBookingFailure
            {
                Id = "failure-1",
            }));

        exception.Message.ShouldBe("Marketplace failure failure-1 has no billable refund owner.");
    }

    [Fact]
    public void Reject_Whitespace_Owner_Ids()
    {
        var exception = Should.Throw<InvalidOperationException>(() =>
            new MarketplaceRefundOwnershipService().Resolve(new MarketplaceBookingFailure
            {
                Id = "failure-1",
                BookingId = " ",
                RecurringBookingId = "\t",
                MarketplaceBookingSubscriptionId = "\r\n",
            }));

        exception.Message.ShouldBe("Marketplace failure failure-1 has no billable refund owner.");
    }
}
