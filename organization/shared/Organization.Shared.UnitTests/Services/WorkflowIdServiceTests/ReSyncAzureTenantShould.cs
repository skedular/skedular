using AutoFixture.Xunit3;
using Enterprise.Shared.Temporal;
using FakeItEasy;
using Organization.Shared.Services;
using Organization.Shared.Workflows;

namespace Organization.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ReSyncAzureTenantShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string tenantId,
        string expectedWorkflowId)
    {
        var rawWorkflowId = $"{Constants.ReSyncAzureTenantPrefix}-{tenantId}";

        A.CallTo(() => temporalHelperService.ToId(rawWorkflowId)).Returns(expectedWorkflowId);

        sut.ReSyncAzureTenant(tenantId).ShouldBe(expectedWorkflowId);
    }
}
