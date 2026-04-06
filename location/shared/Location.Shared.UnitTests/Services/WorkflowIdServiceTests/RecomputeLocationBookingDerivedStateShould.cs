using Enterprise.Shared.Temporal;
using Location.Shared.Services;
using Location.Shared.Workflows;

namespace Location.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RecomputeLocationBookingDerivedStateShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string locationId,
        string expectedWorkflowId)
    {
        var rawWorkflowId = $"{Constants.RecomputeLocationBookingDerivedStatePrefix}-{locationId}";

        A.CallTo(() => temporalHelperService.ToId(rawWorkflowId)).Returns(expectedWorkflowId);

        sut.RecomputeLocationBookingDerivedState(locationId).ShouldBe(expectedWorkflowId);
    }
}
