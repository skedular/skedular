using Enterprise.Shared.Temporal;
using MsTeams.Shared.Services;
using MsTeams.Shared.Workflows;

namespace MsTeams.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ReSyncMsTeamsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string tenantId,
        string expectedWorkflowId)
    {
        var rawWorkflowId = $"{Constants.ReSyncMsTeamsPrefix}-{tenantId}";

        A.CallTo(() => temporalHelperService.ToId(rawWorkflowId)).Returns(expectedWorkflowId);

        sut.ReSyncMsTeams(tenantId).ShouldBe(expectedWorkflowId);
    }
}
