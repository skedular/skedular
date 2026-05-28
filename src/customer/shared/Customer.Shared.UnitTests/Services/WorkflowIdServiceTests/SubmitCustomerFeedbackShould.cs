using Customer.Shared.Services;
using Enterprise.Shared.Temporal;

namespace Customer.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SubmitCustomerFeedbackShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string customerFeedbackId,
        string expectedWorkflowId)
    {
        A.CallTo(() => temporalHelperService.ToId(customerFeedbackId)).Returns(expectedWorkflowId);

        sut.SubmitCustomerFeedback(customerFeedbackId).ShouldBe(expectedWorkflowId);
    }
}
