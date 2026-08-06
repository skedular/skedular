using Api.Shared.Services;
using Booking.Api.GraphQL.MarketplaceBookingSubscription;
using Booking.Api.Services;
using Booking.Shared.Models;
using SubscriptionRootMutation = Booking.Api.GraphQL.MarketplaceBookingSubscription.RootMutation;

namespace Booking.Api.UnitTests.GraphQL.MarketplaceBookingSubscription.RootMutationTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DeleteMarketplaceBookingSubscriptionShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Policy_Restriction_Error(
        [Frozen]
        IMarketplaceBookingSubscriptionService service,
        SubscriptionRootMutation sut,
        CancellationToken cancellationToken)
    {
        var input = new DeleteMarketplaceBookingSubscriptionInput
        {
            Id = "subscription-1",
        };
        A.CallTo(() => service.DeleteAsync(input.Id, input.CancellationMode, input.CancellationOverrideReason, cancellationToken))
            .ThrowsAsync(new MarketplaceBookingSubscriptionCancellationNotAllowed());

        var result = await sut.DeleteMarketplaceBookingSubscriptionAsync(input, service, cancellationToken);

        result.CancellationError!.Code.ShouldBe(CancellationErrorCode.PolicyRestriction);
        result.MarketplaceBookingSubscription.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Override_Reason_Error(
        [Frozen]
        IMarketplaceBookingSubscriptionService service,
        SubscriptionRootMutation sut,
        CancellationToken cancellationToken)
    {
        var input = new DeleteMarketplaceBookingSubscriptionInput
        {
            Id = "subscription-1",
        };
        A.CallTo(() => service.DeleteAsync(input.Id, input.CancellationMode, input.CancellationOverrideReason, cancellationToken))
            .ThrowsAsync(new MarketplaceBookingSubscriptionCancellationOverrideReasonRequired());

        var result = await sut.DeleteMarketplaceBookingSubscriptionAsync(input, service, cancellationToken);

        result.CancellationError!.Code.ShouldBe(CancellationErrorCode.OverrideReasonRequired);
    }
}
