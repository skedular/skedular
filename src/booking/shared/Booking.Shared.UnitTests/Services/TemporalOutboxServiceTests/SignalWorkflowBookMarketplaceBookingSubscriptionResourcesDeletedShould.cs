using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Temporal;
using Temporalio.Client;

namespace Booking.Shared.UnitTests.Services.TemporalOutboxServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SignalWorkflowBookMarketplaceBookingSubscriptionResourcesDeletedShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Enqueue_Signal_With_Correct_Workflow_Id_And_Args(
        [Frozen]
        IWorkflowIdService workflowIdService,
        [Frozen]
        ITemporalSignalOutboxWorkflowExecutor temporalSignalOutboxWorkflowExecutor,
        TemporalOutboxService sut,
        IUnitOfWork unitOfWork,
        string marketplaceBookingSubscriptionId,
        string expectedWorkflowId)
    {
        A.CallTo(() => workflowIdService.BookMarketplaceBookingSubscriptionResources(marketplaceBookingSubscriptionId))
            .Returns(expectedWorkflowId);

        sut.SignalWorkflowBookMarketplaceBookingSubscriptionResourcesDeleted(marketplaceBookingSubscriptionId, unitOfWork);

        A.CallTo(() => temporalSignalOutboxWorkflowExecutor.Signal(
            expectedWorkflowId,
            A<string>._,
            A<MarketplaceBookingSubscriptionDeletedArgs>.That.Matches(a =>
                a.MarketplaceBookingSubscriptionId == marketplaceBookingSubscriptionId),
            A<WorkflowSignalOptions>._,
            unitOfWork)).MustHaveHappenedOnceExactly();
    }
}
