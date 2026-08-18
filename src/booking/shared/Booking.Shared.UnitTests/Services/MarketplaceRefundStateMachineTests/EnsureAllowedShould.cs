using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundStateMachineTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EnsureAllowedShould
{
    [Fact]
    public void Throw_For_Unsupported_Transition()
    {
        var exception = Should.Throw<InvalidOperationException>(() =>
            MarketplaceRefundStateMachine.EnsureAllowed("Rejected", "Processing"));

        exception.Message.ShouldBe("This refund can't be moved from Rejected to Processing.");
    }
}
