using Booking.Shared.Services;
using Enterprise.Shared.Temporal;

namespace Booking.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class BookMarketplaceBookingSubscriptionResourcesShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen]
        ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string marketplaceBookingSubscriptionId,
        string expectedWorkflowId)
    {
        A.CallTo(() => temporalHelperService.ToId(marketplaceBookingSubscriptionId)).Returns(expectedWorkflowId);

        sut.BookMarketplaceBookingSubscriptionResources(marketplaceBookingSubscriptionId).ShouldBe(expectedWorkflowId);
    }
}
