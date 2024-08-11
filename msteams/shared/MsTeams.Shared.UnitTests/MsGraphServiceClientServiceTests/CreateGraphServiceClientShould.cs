using FluentAssertions;
using MsTeams.Shared.Services;
using Testing.Shared;
using Xunit;

namespace MsTeams.Shared.UnitTests.MsGraphServiceClientServiceTests;

public class CreateGraphServiceClientShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_GraphServiceClient_WithCorrect_Configuration(
        MsGraphServiceClientService sut,
        string tenantId)
    {
        var graphServiceClient = sut.CreateGraphServiceClient(tenantId);

        graphServiceClient.Should().NotBeNull();
    }
}
