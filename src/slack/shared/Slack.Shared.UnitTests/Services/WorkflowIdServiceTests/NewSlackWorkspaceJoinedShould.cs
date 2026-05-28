using Enterprise.Shared.Temporal;
using Slack.Shared.Services;
using WorkflowConstants = Slack.Shared.Workflows.Constants;

namespace Slack.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class NewSlackWorkspaceJoinedShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string workspaceId,
        string expectedWorkflowId)
    {
        var rawWorkflowId = $"{WorkflowConstants.NewSlackWorkspaceJoinedPrefix}-{workspaceId}";

        A.CallTo(() => temporalHelperService.ToId(rawWorkflowId)).Returns(expectedWorkflowId);

        sut.NewSlackWorkspaceJoined(workspaceId).ShouldBe(expectedWorkflowId);
    }
}
