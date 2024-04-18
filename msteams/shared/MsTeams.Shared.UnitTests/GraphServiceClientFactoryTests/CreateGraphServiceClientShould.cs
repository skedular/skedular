using FluentAssertions;
using MsTeams.Shared.Factories;
using Testing.Shared;
using Xunit;

namespace MsTeams.Shared.UnitTests.GraphServiceClientFactoryTests;

public class CreateGraphServiceClientShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_GraphServiceClient_WithCorrect_Configuration(
        CreateGraphServiceClientFactory sut,
        string tenantId)
    {
        var graphServiceClient = sut.CreateGraphServiceClient(tenantId);

        graphServiceClient.Should().NotBeNull();
    }
}
