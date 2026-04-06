using Customer.Shared.Services;
using Customer.Shared.Workflows;
using Enterprise.Shared.Temporal;

namespace Customer.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class NewCustomerJoinedShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string customerId,
        string expectedWorkflowId)
    {
        var rawWorkflowId = $"{Constants.NewCustomerJoinedPrefix}-{customerId}";

        A.CallTo(() => temporalHelperService.ToId(rawWorkflowId)).Returns(expectedWorkflowId);

        sut.NewCustomerJoined(customerId).ShouldBe(expectedWorkflowId);
    }
}
