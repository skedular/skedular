using Microsoft.Extensions.Logging;
using Team.Api.GraphQL.Invitation;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Shared.Models;

namespace Team.Api.UnitTests.GraphQL.Invitation.RootMutationTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AcceptInvitationToJoinTeamAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_information_for_accept_invitation_mutation(
        [Frozen] IMapper mapper,
        [Frozen] IInvitationService invitationService,
        [Frozen] ILogger<RootMutation> logger,
        RootMutation sut,
        AcceptInvitationToJoinTeamInput input,
        JoinInvitation invitation,
        CancellationToken cancellationToken)
    {
        var invitationDetails = new InviteCustomerToJoinTeamDetails();

        A.CallTo(() => invitationService.AcceptInvitationToJoinAsync(input.Id, cancellationToken)).Returns(invitation);
        A.CallTo(() => mapper.MapTo(invitation)).Returns(invitationDetails);

        await sut.AcceptInvitationToJoinTeamAsync(input, invitationService, cancellationToken);

        var logCalls = Fake.GetCalls(logger)
            .Where(call => call.Method.Name == nameof(ILogger.Log))
            .ToList();

        logCalls.Count(call => Equals(call.Arguments[0], LogLevel.Information)).ShouldBe(2);
    }
}
