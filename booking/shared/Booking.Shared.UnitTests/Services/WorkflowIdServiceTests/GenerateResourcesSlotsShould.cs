using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Temporal;

namespace Booking.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GenerateResourcesSlotsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string locationId,
        string expectedWorkflowId)
    {
        var rawWorkflowId = $"{Constants.GenerateResourcesSlotsPrefix}-{locationId}";

        A.CallTo(() => temporalHelperService.ToId(rawWorkflowId)).Returns(expectedWorkflowId);

        sut.GenerateResourcesSlots(locationId).ShouldBe(expectedWorkflowId);
    }
}
