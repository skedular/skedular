using HotChocolate;
using HotChocolate.Types;
using Location.Api.Mappers;
using Location.Api.Services;

namespace Location.Api.GraphQL;

[MutationType]
public class Mutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<LocationPayload?> AddLocationAsync(
        AddLocationInput input,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken)
    {
        var location = await locationService.AddAsync(mapper.MapTo(input), false, cancellationToken);
        return new LocationPayload { ClientMutationId = input.ClientMutationId, Location = mapper.MapTo(location)! };
    }

    [UseResolverScope]
    public async Task<LocationPayload?> UpdateLocationAsync(
        UpdateLocationInput input,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken)
    {
        var location = await locationService.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new LocationPayload { ClientMutationId = input.ClientMutationId, Location = mapper.MapTo(location)! };
    }

    [UseResolverScope]
    public async Task<LocationPayload?> DeleteLocationAsync(
        DeleteLocationInput input,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken)
    {
        var location = await locationService.DeleteAsync(input.Id, cancellationToken);
        return new LocationPayload { ClientMutationId = input.ClientMutationId, Location = mapper.MapTo(location)! };
    }

    [UseResolverScope]
    public async Task<DeskPayload?> AddDeskAsync(
        AddDeskInput input,
        [Service] IDeskService deskService,
        CancellationToken cancellationToken)
    {
        var desk = await deskService.AddAsync(mapper.MapTo(input), false, cancellationToken);
        return new DeskPayload { ClientMutationId = input.ClientMutationId, Desk = mapper.MapTo(desk) };
    }

    [UseResolverScope]
    public async Task<BulkDeskPayload?> BulkAddDeskAsync(
        BulkAddDeskInput input,
        [Service] IDeskService deskService,
        CancellationToken cancellationToken)
    {
        var desks = await deskService.BulkAddAsync(
            input.LocationId,
            input.NamePrefix,
            input.Count,
            input.CustomTagIds,
            input.ZoneIds,
            input.Deactivated,
            input.RequireBookingApproval,
            cancellationToken);
        return new BulkDeskPayload
        {
            ClientMutationId = input.ClientMutationId, Desks = desks.Select(mapper.MapTo).ToArray()
        };
    }

    [UseResolverScope]
    public async Task<DeskPayload?> UpdateDeskAsync(
        UpdateDeskInput input,
        [Service] IDeskService deskService,
        CancellationToken cancellationToken)
    {
        var desk = await deskService.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new DeskPayload { ClientMutationId = input.ClientMutationId, Desk = mapper.MapTo(desk) };
    }

    [UseResolverScope]
    public async Task<DeskPayload?> DeleteDeskAsync(
        DeleteDeskInput input,
        [Service] IDeskService deskService,
        CancellationToken cancellationToken)
    {
        var desk = await deskService.DeleteAsync(input.Id, cancellationToken);
        return new DeskPayload { ClientMutationId = input.ClientMutationId, Desk = mapper.MapTo(desk) };
    }

    [UseResolverScope]
    public async Task<DesksPayload?> DeleteDesksAsync(
        DeleteDesksInput input,
        [Service] IDeskService deskService,
        CancellationToken cancellationToken)
    {
        var desks = await deskService.DeleteAsync(input.Ids, cancellationToken);
        return new DesksPayload
        {
            ClientMutationId = input.ClientMutationId, Desks = desks.Select(mapper.MapTo).ToArray()
        };
    }

    [UseResolverScope]
    public async Task<DesksPayload?> ActivateDesksAsync(
        ActivateDesksInput input,
        [Service] IDeskService deskService,
        CancellationToken cancellationToken)
    {
        var desks = await deskService.ActivateAsync(input.Ids, cancellationToken);
        return new DesksPayload
        {
            ClientMutationId = input.ClientMutationId, Desks = desks.Select(mapper.MapTo).ToArray()
        };
    }

    [UseResolverScope]
    public async Task<DesksPayload?> DeactivateDesksAsync(
        DeactivateDesksInput input,
        [Service] IDeskService deskService,
        CancellationToken cancellationToken)
    {
        var desks = await deskService.DeactivateAsync(input.Ids, cancellationToken);
        return new DesksPayload
        {
            ClientMutationId = input.ClientMutationId, Desks = desks.Select(mapper.MapTo).ToArray()
        };
    }

    [UseResolverScope]
    public async Task<LocationMemberDetailsPayload?> ChangeLocationMemberRoleAsync(
        ChangeLocationMemberRoleInput input,
        [Service] ILocationMemberService locationMemberService,
        CancellationToken cancellationToken)
    {
        var locationMember =
            await locationMemberService.ChangeRoleAsync(
                input.Id,
                input.Role,
                cancellationToken);
        return new LocationMemberDetailsPayload
        {
            ClientMutationId = input.ClientMutationId, Member = mapper.MapTo(locationMember)
        };
    }

    [UseResolverScope]
    public async Task<InviteCustomersToJoinLocationPayload?> InviteCustomersToJoinLocationAsync(
        InviteCustomersToJoinLocationInput input,
        [Service] ILocationInvitationService locationInvitationService,
        CancellationToken cancellationToken)
    {
        await locationInvitationService.InviteMembersByEmailsAsync(input.LocationId, input.Emails, cancellationToken);
        return new InviteCustomersToJoinLocationPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<AcceptInvitationToJoinLocationPayload?> AcceptInvitationToJoinLocationAsync(
        AcceptInvitationToJoinLocationInput input,
        [Service] ILocationInvitationService locationInvitationService,
        CancellationToken cancellationToken)
    {
        await locationInvitationService.AcceptInvitationToJoinAsync(input.Id, cancellationToken);
        return new AcceptInvitationToJoinLocationPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<RejectInvitationToJoinLocationPayload?> RejectInvitationToJoinLocationAsync(
        RejectInvitationToJoinLocationInput input,
        [Service] ILocationInvitationService locationInvitationService,
        CancellationToken cancellationToken)
    {
        await locationInvitationService.RejectInvitationToJoinAsync(input.Id, cancellationToken);
        return new RejectInvitationToJoinLocationPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<CancelInvitationToJoinLocationPayload?> CancelInvitationToJoinLocationAsync(
        CancelInvitationToJoinLocationInput input,
        [Service] ILocationInvitationService locationInvitationService,
        CancellationToken cancellationToken)
    {
        await locationInvitationService.CancelInvitationToJoinAsync(input.Id, cancellationToken);
        return new CancelInvitationToJoinLocationPayload { ClientMutationId = input.ClientMutationId };
    }
}
