using Microsoft.Extensions.Logging;
using Team.Api.GraphQL.Member;
using Team.Api.GraphQL.Team;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Shared.Models;
using RootMutation = Team.Api.GraphQL.Member.RootMutation;

namespace Team.Api.UnitTests.GraphQL.Member.RootMutationTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdateTeamMembersAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_information_for_update_team_members_mutation(
        [Frozen] IMapper mapper,
        [Frozen] ITeamMemberService teamMemberService,
        [Frozen] ILogger<RootMutation> logger,
        RootMutation sut,
        UpdateTeamMembersInput input,
        Shared.Models.Team team,
        CancellationToken cancellationToken)
    {
        var mappedMembers = new List<TeamMember>();
        var teamDetails = new TeamDetails();

        A.CallTo(() => mapper.MapToTeamMembers(input)).Returns(mappedMembers);
        A.CallTo(() => teamMemberService.UpdateMembersAsync(input.Id, mappedMembers, cancellationToken)).Returns(team);
        A.CallTo(() => mapper.MapTo(team)).Returns(teamDetails);

        await sut.UpdateTeamMembersAsync(input, teamMemberService, cancellationToken);

        var logCalls = Fake.GetCalls(logger)
            .Where(call => call.Method.Name == nameof(ILogger.Log))
            .ToList();

        logCalls.Count(call => Equals(call.Arguments[0], LogLevel.Information)).ShouldBe(2);
    }
}
