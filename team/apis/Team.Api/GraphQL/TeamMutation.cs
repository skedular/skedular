using Api.Shared.Models;
using Api.Shared.Services.GraphQL.UnityHub.V1.Team;
using Enterprise.Shared.Context;
using Team.Api.Mappers;
using Team.Api.Services;

namespace Team.Api.GraphQL;

public class TeamMutation(IMapper mapper) : Mutation
{
    public override async Task<TeamPayload?> AddTeamAsync(
        AddTeamInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ITeamService>();
        var team = await service.AddAsync(mapper.MapTo(input), false, cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }

    public override async Task<TeamPayload?> UpdateTeamAsync(
        UpdateTeamInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ITeamService>();
        var team = await service.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }

    public override async Task<TeamPayload?> DeleteTeamAsync(
        DeleteTeamInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ITeamService>();
        var team = await service.DeleteAsync(input.Id, cancellationToken);
        return new TeamPayload { ClientMutationId = input.ClientMutationId, Team = mapper.MapTo(team)! };
    }

    public override async Task<TeamMemberDetailsPayload?> ChangeTeamMemberOwnershipTypeAsync(
        ChangeTeamMemberOwnershipTypeInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ITeamMemberService>();
        var teamMember =
            await service.ChangeMembershipTypeAsync(
                input.Id,
                input.MembershipType switch
                {
                    TeamMemberMembershipType.OWNER => TeamMembershipType.Owner,
                    TeamMemberMembershipType.ADMINISTRATOR => TeamMembershipType.Administrator,
                    TeamMemberMembershipType.MEMBER => TeamMembershipType.Member,
                    _ => throw new ArgumentOutOfRangeException()
                },
                cancellationToken);
        return new TeamMemberDetailsPayload
        {
            ClientMutationId = input.ClientMutationId, Member = mapper.MapTo(teamMember)
        };
    }

    public override async Task<InviteCustomersToJoinTeamPayload?> InviteCustomersToJoinTeamAsync(
        InviteCustomersToJoinTeamInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ITeamInvitationService>();
        await service.InviteMembersByEmailsAsync(input.TeamId, input.Emails, cancellationToken);
        return new InviteCustomersToJoinTeamPayload { ClientMutationId = input.ClientMutationId };
    }

    public override async Task<AcceptInvitationToJoinTeamPayload?> AcceptInvitationToJoinTeamAsync(
        AcceptInvitationToJoinTeamInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ITeamInvitationService>();
        await service.AcceptInvitationToJoinAsync(input.Id, cancellationToken);
        return new AcceptInvitationToJoinTeamPayload { ClientMutationId = input.ClientMutationId };
    }

    public override async Task<RejectInvitationToJoinTeamPayload?> RejectInvitationToJoinTeamAsync(
        RejectInvitationToJoinTeamInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ITeamInvitationService>();
        await service.RejectInvitationToJoinAsync(input.Id, cancellationToken);
        return new RejectInvitationToJoinTeamPayload { ClientMutationId = input.ClientMutationId };
    }

    public override async Task<CancelInvitationToJoinTeamPayload?> CancelInvitationToJoinTeamAsync(
        CancelInvitationToJoinTeamInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ITeamInvitationService>();
        await service.CancelInvitationToJoinAsync(input.Id, cancellationToken);
        return new CancelInvitationToJoinTeamPayload { ClientMutationId = input.ClientMutationId };
    }
}
