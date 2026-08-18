using Booking.Shared.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundStateMachineTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CanTransitionShould
{
    [Fact]
    public void Allow_Supported_Transitions()
    {
        var supported = new[]
        {
            (MarketplaceRefundStatusConstants.Requested, MarketplaceRefundStatusConstants.UnderReview),
            (MarketplaceRefundStatusConstants.Requested, MarketplaceRefundStatusConstants.Processing),
            (MarketplaceRefundStatusConstants.Requested, MarketplaceRefundStatusConstants.Cancelled),
            (MarketplaceRefundStatusConstants.UnderReview, MarketplaceRefundStatusConstants.Approved),
            (MarketplaceRefundStatusConstants.UnderReview, MarketplaceRefundStatusConstants.Rejected),
            (MarketplaceRefundStatusConstants.Approved, MarketplaceRefundStatusConstants.Processing),
            (MarketplaceRefundStatusConstants.Processing, MarketplaceRefundStatusConstants.Completed),
            (MarketplaceRefundStatusConstants.Processing, MarketplaceRefundStatusConstants.Failed),
            (MarketplaceRefundStatusConstants.Processing, MarketplaceRefundStatusConstants.ReconciliationRequired),
            (MarketplaceRefundStatusConstants.ProviderPending, MarketplaceRefundStatusConstants.Completed),
            (MarketplaceRefundStatusConstants.Failed, MarketplaceRefundStatusConstants.Processing),
            (MarketplaceRefundStatusConstants.ReconciliationRequired, MarketplaceRefundStatusConstants.Completed),
            (MarketplaceRefundStatusConstants.Completed, MarketplaceRefundStatusConstants.ReconciliationRequired),
        };

        foreach (var (current, next) in supported)
        {
            MarketplaceRefundStateMachine.CanTransition(current, next).ShouldBeTrue();
        }
    }

    [Theory]
    [InlineData("Rejected", "Processing")]
    [InlineData("Cancelled", "Processing")]
    [InlineData("Requested", "Completed")]
    [InlineData("UnderReview", "Processing")]
    [InlineData("Approved", "Completed")]
    [InlineData("Approved", "Cancelled")]
    [InlineData("ProviderPending", "Processing")]
    [InlineData("Completed", "Processing")]
    [InlineData("Unknown", "Processing")]
    public void Reject_Unsupported_Transitions(string current, string next) =>
        MarketplaceRefundStateMachine.CanTransition(current, next).ShouldBeFalse();

    [Theory]
    [InlineData("Requested")]
    [InlineData("Completed")]
    [InlineData("Rejected")]
    [InlineData("Cancelled")]
    public void Allow_Idempotent_Transitions(string status) => MarketplaceRefundStateMachine.CanTransition(status, status).ShouldBeTrue();
}
