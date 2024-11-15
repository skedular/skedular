using Api.Shared.Models;
using Enterprise.Shared.Context;
using Location.Api.Mappers;
using Location.Api.Services;

namespace Location.Api.GraphQL;

public class LocationMutation(IServiceProvider serviceProvider, IMapper mapper)
{
    public async Task<LocationPayload?> AddLocationAsync(
        AddLocationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ILocationService>();
        var location = await service.AddAsync(mapper.MapTo(input), false, cancellationToken);
        return new LocationPayload { ClientMutationId = input.ClientMutationId, Location = mapper.MapTo(location)! };
    }

    public async Task<LocationPayload?> UpdateLocationAsync(
        UpdateLocationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ILocationService>();
        var location = await service.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new LocationPayload { ClientMutationId = input.ClientMutationId, Location = mapper.MapTo(location)! };
    }

    public async Task<LocationPayload?> DeleteLocationAsync(
        DeleteLocationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ILocationService>();
        var location = await service.DeleteAsync(input.Id, cancellationToken);
        return new LocationPayload { ClientMutationId = input.ClientMutationId, Location = mapper.MapTo(location)! };
    }

    public async Task<DeskPayload?> AddDeskAsync(
        AddDeskInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IDeskService>();
        var desk = await service.AddAsync(mapper.MapTo(input), false, cancellationToken);
        return new DeskPayload { ClientMutationId = input.ClientMutationId, Desk = mapper.MapTo(desk) };
    }

    public async Task<BulkDeskPayload?> BulkAddDeskAsync(
        BulkAddDeskInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IDeskService>();
        var desks = await service.BulkAddAsync(
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

    public async Task<DeskPayload?> UpdateDeskAsync(
        UpdateDeskInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IDeskService>();
        var desk = await service.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new DeskPayload { ClientMutationId = input.ClientMutationId, Desk = mapper.MapTo(desk) };
    }

    public async Task<DeskPayload?> DeleteDeskAsync(
        DeleteDeskInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IDeskService>();
        var desk = await service.DeleteAsync(input.Id, cancellationToken);
        return new DeskPayload { ClientMutationId = input.ClientMutationId, Desk = mapper.MapTo(desk) };
    }

    public async Task<LocationTagPayload?> AddLocationTagAsync(
        AddLocationTagInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ITagService>();
        var tag = await service.AddAsync(mapper.MapTo(input), false, cancellationToken);
        return new LocationTagPayload { ClientMutationId = input.ClientMutationId, LocationTag = mapper.MapTo(tag) };
    }

    public async Task<LocationTagPayload?> UpdateLocationTagAsync(
        UpdateLocationTagInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ITagService>();
        var tag = await service.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new LocationTagPayload { ClientMutationId = input.ClientMutationId, LocationTag = mapper.MapTo(tag) };
    }

    public async Task<LocationTagPayload?> DeleteLocationTagAsync(
        DeleteLocationTagInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ITagService>();
        var tag = await service.DeleteAsync(input.Id, cancellationToken);
        return new LocationTagPayload { ClientMutationId = input.ClientMutationId, LocationTag = mapper.MapTo(tag) };
    }

    public async Task<LocationMemberDetailsPayload?> ChangeLocationMemberOwnershipTypeAsync(
        ChangeLocationMemberOwnershipTypeInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ILocationMemberService>();
        var locationMember =
            await service.ChangeMembershipTypeAsync(
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

    public async Task<InviteCustomersToJoinLocationPayload?> InviteCustomersToJoinLocationAsync(
        InviteCustomersToJoinLocationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ILocationInvitationService>();
        await service.InviteMembersByEmailsAsync(input.LocationId, input.Emails, cancellationToken);
        return new InviteCustomersToJoinLocationPayload { ClientMutationId = input.ClientMutationId };
    }

    public async Task<AcceptInvitationToJoinLocationPayload?> AcceptInvitationToJoinLocationAsync(
        AcceptInvitationToJoinLocationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ILocationInvitationService>();
        await service.AcceptInvitationToJoinAsync(input.Id, cancellationToken);
        return new AcceptInvitationToJoinLocationPayload { ClientMutationId = input.ClientMutationId };
    }

    public async Task<RejectInvitationToJoinLocationPayload?> RejectInvitationToJoinLocationAsync(
        RejectInvitationToJoinLocationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ILocationInvitationService>();
        await service.RejectInvitationToJoinAsync(input.Id, cancellationToken);
        return new RejectInvitationToJoinLocationPayload { ClientMutationId = input.ClientMutationId };
    }

    public async Task<CancelInvitationToJoinLocationPayload?> CancelInvitationToJoinLocationAsync(
        CancelInvitationToJoinLocationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<ILocationInvitationService>();
        await service.CancelInvitationToJoinAsync(input.Id, cancellationToken);
        return new CancelInvitationToJoinLocationPayload { ClientMutationId = input.ClientMutationId };
    }
}
