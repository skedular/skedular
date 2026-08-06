using Microsoft.Extensions.Logging;
using Team.Api.GraphQL.Team;
using Team.Api.Mappers;
using Team.Api.Services;

namespace Team.Api.UnitTests.GraphQL.Team.RootMutationTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddTeamAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_information_for_add_team_mutation(
        [Frozen]
        IGraphQlMapper graphQlMapper,
        [Frozen]
        ITeamService teamService,
        [Frozen]
        ILogger<RootMutation> logger,
        RootMutation sut,
        AddTeamInput input,
        Shared.Models.Team team,
        CancellationToken cancellationToken)
    {
        var teamDetails = new TeamDetails();

        A.CallTo(() => graphQlMapper.MapTo(input)).Returns(team);
        A.CallTo(() => teamService.AddAsync(team, cancellationToken)).Returns(team);
        A.CallTo(() => graphQlMapper.MapTo(team)).Returns(teamDetails);

        await sut.AddTeamAsync(input, teamService, cancellationToken);

        var logCalls = Fake.GetCalls(logger).Where(call => call.Method.Name == nameof(ILogger.Log)).ToList();

        logCalls.Count(call => Equals(call.Arguments[0], LogLevel.Information)).ShouldBe(2);
    }
}
