using Enterprise.Shared.Configurations;
using Enterprise.Shared.Temporal;
using Temporalio.Client;

namespace Enterprise.Shared.UnitTests.Temporal.TemporalHelperServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ToIdShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Original_Id_When_Environment_Is_Null(
        ITemporalClient temporalClient,
        string workflowId)
    {
        var sut = new TemporalHelperService(new ApplicationConfiguration(), temporalClient);

        sut.ToId(workflowId).ShouldBe(workflowId);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_The_Original_Id_When_Environment_Is_Whitespace(
        ITemporalClient temporalClient,
        string workflowId)
    {
        var sut = new TemporalHelperService(
            new ApplicationConfiguration { Environment = "   " },
            temporalClient);

        sut.ToId(workflowId).ShouldBe(workflowId);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Prefix_The_Id_When_Environment_Is_Set(
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
            new ApplicationConfiguration { Environment = environment },
            temporalClient);

        sut.ToId(workflowId).ShouldBe($"{environment}.{workflowId}");
    }
}
