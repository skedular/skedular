using HotChocolate;
using HotChocolate.Types;
using Team.Api.Mappers;
using Team.Api.Services;

namespace Team.Api.GraphQL.Invitation;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper, ILogger<RootMutation> logger)
{
    [UseResolverScope]
    public async Task<InviteCustomersToJoinTeamPayload> InviteCustomersToJoinTeamAsync(
        InviteCustomersToJoinTeamInput input,
        [Service]
        IInvitationService invitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InvitesCustomersToJoinTeam = graphQlMapper.MapTo(
                    await invitationService.InviteMembersByEmailsAsync(input.TeamId, input.Emails.ToList(), cancellationToken))
                .ToList(),
        };

    [UseResolverScope]
    public async Task<InvitationToJoinTeamPayload> AcceptInvitationToJoinTeamAsync(
        AcceptInvitationToJoinTeamInput input,
        [Service]
        IInvitationService invitationService,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting {OperationName}", nameof(AcceptInvitationToJoinTeamAsync));
        var invitation = await invitationService.AcceptInvitationToJoinAsync(input.Id, cancellationToken);
        logger.LogInformation("Completed {OperationName}", nameof(AcceptInvitationToJoinTeamAsync));

        return new InvitationToJoinTeamPayload
        {
            ClientMutationId = input.ClientMutationId,
            InviteCustomerToJoinTeam = graphQlMapper.MapTo(invitation),
        };
    }

    [UseResolverScope]
    public async Task<InvitationToJoinTeamPayload> RejectInvitationToJoinTeamAsync(
        RejectInvitationToJoinTeamInput input,
        [Service]
        IInvitationService invitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InviteCustomerToJoinTeam = graphQlMapper.MapTo(await invitationService.RejectInvitationToJoinAsync(input.Id, cancellationToken)),
        };

    [UseResolverScope]
    public async Task<InvitationToJoinTeamPayload> CancelInvitationToJoinTeamAsync(
        CancelInvitationToJoinTeamInput input,
        [Service]
        IInvitationService invitationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            InviteCustomerToJoinTeam = graphQlMapper.MapTo(await invitationService.CancelInvitationToJoinAsync(input.Id, cancellationToken)),
        };
}
