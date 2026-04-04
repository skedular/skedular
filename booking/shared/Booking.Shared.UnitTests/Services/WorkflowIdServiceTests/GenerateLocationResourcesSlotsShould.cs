using AutoFixture.Xunit3;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Temporal;
using FakeItEasy;

namespace Booking.Shared.UnitTests.Services.WorkflowIdServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GenerateLocationResourcesSlotsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Deterministic_Workflow_Id(
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowIdService sut,
        string locationId,
        string expectedWorkflowId)
    {
        var rawWorkflowId = $"{Constants.GenerateLocationResourcesSlotsPrefix}-{locationId}";

        A.CallTo(() => temporalHelperService.ToId(rawWorkflowId)).Returns(expectedWorkflowId);

        sut.GenerateLocationResourcesSlots(locationId).ShouldBe(expectedWorkflowId);
    }
}
