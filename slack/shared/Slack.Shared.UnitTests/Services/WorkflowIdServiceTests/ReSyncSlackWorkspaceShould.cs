using AutoFixture.Xunit3;
using Enterprise.Shared.Temporal;
using FakeItEasy;
using Slack.Shared.Services;
using WorkflowConstants = Slack.Shared.Workflows.Constants;

namespace Slack.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ReSyncSlackWorkspaceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string workspaceId,
        string expectedWorkflowId)
    {
        var rawWorkflowId = $"{WorkflowConstants.ReSyncSlackWorkspacePrefix}-{workspaceId}";

        A.CallTo(() => temporalHelperService.ToId(rawWorkflowId)).Returns(expectedWorkflowId);

        sut.ReSyncSlackWorkspace(workspaceId).ShouldBe(expectedWorkflowId);
    }
}
