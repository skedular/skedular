using Api.Shared.Models;
using HotChocolate;
using HotChocolate.Types;
using Location.Api.Mappers;
using Location.Api.Services;

namespace Location.Api.GraphQL;

public class LocationMutation
{
    [UseServiceScope]
    public async Task<LocationPayload?> AddLocationAsync(
        AddLocationInput input,
        [Service] ILocationService locationService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var location = await locationService.AddAsync(mapper.MapTo(input), false, cancellationToken);
        return new LocationPayload { ClientMutationId = input.ClientMutationId, Location = mapper.MapTo(location)! };
    }

    [UseServiceScope]
    public async Task<LocationPayload?> UpdateLocationAsync(
        UpdateLocationInput input,
        [Service] ILocationService locationService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var location = await locationService.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new LocationPayload { ClientMutationId = input.ClientMutationId, Location = mapper.MapTo(location)! };
    }

    [UseServiceScope]
    public async Task<LocationPayload?> DeleteLocationAsync(
        DeleteLocationInput input,
        [Service] ILocationService locationService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var location = await locationService.DeleteAsync(input.Id, cancellationToken);
        return new LocationPayload { ClientMutationId = input.ClientMutationId, Location = mapper.MapTo(location)! };
    }

    [UseServiceScope]
    public async Task<DeskPayload?> AddDeskAsync(
        AddDeskInput input,
        [Service] IDeskService deskService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var desk = await deskService.AddAsync(mapper.MapTo(input), false, cancellationToken);
        return new DeskPayload { ClientMutationId = input.ClientMutationId, Desk = mapper.MapTo(desk) };
    }

    [UseServiceScope]
    public async Task<BulkDeskPayload?> BulkAddDeskAsync(
        BulkAddDeskInput input,
        [Service] IDeskService deskService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var desks = await deskService.BulkAddAsync(
            input.LocationId,
            input.NamePrefix,
            input.Count,
            input.LocationTagIds,
            input.Deactivated,
            input.RequireBookingApproval,
            cancellationToken);
        return new BulkDeskPayload
        {
            ClientMutationId = input.ClientMutationId, Desks = desks.Select(mapper.MapTo).ToArray()
        };
    }

    [UseServiceScope]
    public async Task<DeskPayload?> UpdateDeskAsync(
        UpdateDeskInput input,
        [Service] IDeskService deskService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var desk = await deskService.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new DeskPayload { ClientMutationId = input.ClientMutationId, Desk = mapper.MapTo(desk) };
    }

    [UseServiceScope]
    public async Task<DeskPayload?> DeleteDeskAsync(
        DeleteDeskInput input,
        [Service] IDeskService deskService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var desk = await deskService.DeleteAsync(input.Id, cancellationToken);
        return new DeskPayload { ClientMutationId = input.ClientMutationId, Desk = mapper.MapTo(desk) };
    }

    [UseServiceScope]
    public async Task<LocationTagPayload?> AddLocationTagAsync(
        AddLocationTagInput input,
        [Service] ITagService tagService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var tag = await tagService.AddAsync(mapper.MapTo(input), false, cancellationToken);
        return new LocationTagPayload { ClientMutationId = input.ClientMutationId, LocationTag = mapper.MapTo(tag) };
    }

    [UseServiceScope]
    public async Task<LocationTagPayload?> UpdateLocationTagAsync(
        UpdateLocationTagInput input,
        [Service] ITagService tagService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var tag = await tagService.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new LocationTagPayload { ClientMutationId = input.ClientMutationId, LocationTag = mapper.MapTo(tag) };
    }

    [UseServiceScope]
    public async Task<LocationTagPayload?> DeleteLocationTagAsync(
        DeleteLocationTagInput input,
        [Service] ITagService tagService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var tag = await tagService.DeleteAsync(input.Id, cancellationToken);
        return new LocationTagPayload { ClientMutationId = input.ClientMutationId, LocationTag = mapper.MapTo(tag) };
    }

    [UseServiceScope]
    public async Task<LocationMemberDetailsPayload?> ChangeLocationMemberOwnershipTypeAsync(
        ChangeLocationMemberOwnershipTypeInput input,
        [Service] ILocationMemberService locationMemberService,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
    {
        var locationMember =
            await locationMemberService.ChangeMembershipTypeAsync(
                input.Id,
                input.MembershipType switch
                {
                    LocationMemberMembershipType.OWNER => LocationMembershipType.Owner,
                    LocationMemberMembershipType.ADMINISTRATOR => LocationMembershipType.Administrator,
                    LocationMemberMembershipType.MEMBER => LocationMembershipType.Member,
                    _ => throw new ArgumentOutOfRangeException()
                },
                cancellationToken);
        return new LocationMemberDetailsPayload
        {
            ClientMutationId = input.ClientMutationId, Member = mapper.MapTo(locationMember)
        };
    }

    [UseServiceScope]
    public async Task<InviteCustomersToJoinLocationPayload?> InviteCustomersToJoinLocationAsync(
        InviteCustomersToJoinLocationInput input,
        [Service] ILocationInvitationService locationInvitationService,
        CancellationToken cancellationToken)
    {
        await locationInvitationService.InviteMembersByEmailsAsync(input.LocationId, input.Emails, cancellationToken);
        return new InviteCustomersToJoinLocationPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseServiceScope]
    public async Task<AcceptInvitationToJoinLocationPayload?> AcceptInvitationToJoinLocationAsync(
        AcceptInvitationToJoinLocationInput input,
        [Service] ILocationInvitationService locationInvitationService,
        CancellationToken cancellationToken)
    {
        await locationInvitationService.AcceptInvitationToJoinAsync(input.Id, cancellationToken);
        return new AcceptInvitationToJoinLocationPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseServiceScope]
    public async Task<RejectInvitationToJoinLocationPayload?> RejectInvitationToJoinLocationAsync(
        RejectInvitationToJoinLocationInput input,
        [Service] ILocationInvitationService locationInvitationService,
        CancellationToken cancellationToken)
    {
        await locationInvitationService.RejectInvitationToJoinAsync(input.Id, cancellationToken);
        return new RejectInvitationToJoinLocationPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseServiceScope]
    public async Task<CancelInvitationToJoinLocationPayload?> CancelInvitationToJoinLocationAsync(
        CancelInvitationToJoinLocationInput input,
        [Service] ILocationInvitationService locationInvitationService,
        CancellationToken cancellationToken)
    {
        await locationInvitationService.CancelInvitationToJoinAsync(input.Id, cancellationToken);
        return new CancelInvitationToJoinLocationPayload { ClientMutationId = input.ClientMutationId };
    }
}
