using Enterprise.Shared.Temporal;
using Organization.Shared.Services;

namespace Organization.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class InviteToJoinShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string joinInvitationId,
        string expectedWorkflowId)
    {
        A.CallTo(() => temporalHelperService.ToId(joinInvitationId)).Returns(expectedWorkflowId);

        sut.InviteToJoin(joinInvitationId).ShouldBe(expectedWorkflowId);
    }
}
