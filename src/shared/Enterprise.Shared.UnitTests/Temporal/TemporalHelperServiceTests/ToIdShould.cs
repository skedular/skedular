using Enterprise.Shared.Configurations;
using Enterprise.Shared.Temporal;
using Microsoft.Extensions.Logging;
using Temporalio.Client;

namespace Enterprise.Shared.UnitTests.Temporal.TemporalHelperServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ToIdShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Original_Id_When_Environment_Is_Null(
        ILogger<TemporalHelperService> logger,
        ITemporalClient temporalClient,
        string workflowId)
    {
        var sut = new TemporalHelperService(new ApplicationConfiguration(), temporalClient, logger);

        sut.ToId(workflowId).ShouldBe(workflowId);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Original_Id_When_Environment_Is_Whitespace(
        ILogger<TemporalHelperService> logger,
        ITemporalClient temporalClient,
        string workflowId)
    {
        var sut = new TemporalHelperService(
            new ApplicationConfiguration
            {
                Environment = "   ",
            },
            temporalClient,
            logger);

        sut.ToId(workflowId).ShouldBe(workflowId);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Prefix_The_Id_When_Environment_Is_Set(
        ILogger<TemporalHelperService> logger,
        ITemporalClient temporalClient,
        string workflowId,
        string environment)
    {
        environment = environment.Trim();

        if (string.IsNullOrWhiteSpace(environment))
        {
            environment = "local";
        }

        var sut = new TemporalHelperService(
            new ApplicationConfiguration
            {
                Environment = environment,
            },
            temporalClient,
            logger);

        sut.ToId(workflowId).ShouldBe($"{environment}.{workflowId}");
    }
}
