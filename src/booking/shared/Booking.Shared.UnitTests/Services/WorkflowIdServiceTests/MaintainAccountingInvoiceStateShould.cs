using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Temporal;

namespace Booking.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MaintainAccountingInvoiceStateShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string localEntityType,
        string localEntityId,
        string expectedWorkflowId)
    {
        var rawWorkflowId = $"{Constants.MaintainAccountingInvoiceStatePrefix}-{localEntityType}-{localEntityId}";

        A.CallTo(() => temporalHelperService.ToId(rawWorkflowId)).Returns(expectedWorkflowId);

        sut.MaintainAccountingInvoiceState(localEntityType, localEntityId).ShouldBe(expectedWorkflowId);
    }
}
