using Enterprise.Shared.Sanitization;
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
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = mapper.MapTo(await locationService.AddAsync(mapper.MapTo(input), false, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<LocationPayload?> UpdateLocationAsync(
        UpdateLocationInput input,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = mapper.MapTo(await locationService.UpdateAsync(mapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<LocationPayload?> DeleteLocationAsync(
        DeleteLocationInput input,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        new() { ClientMutationId = input.ClientMutationId, Location = mapper.MapTo(await locationService.DeleteAsync(input.Id, cancellationToken))! };

    [UseResolverScope]
    public async Task<LocationMemberDetailsPayload?> ChangeLocationMemberRoleAsync(
        ChangeLocationMemberRoleInput input,
        [Service] ILocationMemberService locationMemberService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Member = mapper.MapTo(await locationMemberService.ChangeRoleAsync(input.Id, input.Role, cancellationToken))
        };

    [UseResolverScope]
    public async Task<InviteCustomersToJoinLocationPayload?> InviteCustomersToJoinLocationAsync(
        InviteCustomersToJoinLocationInput input,
        [Service] ILocationInvitationService locationInvitationService,
        CancellationToken cancellationToken)
    {
        await locationInvitationService.InviteMembersByEmailsAsync(input.LocationId, input.Emails.ToList(), cancellationToken);
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

    [UseResolverScope]
    public async Task<ResourcePayload?> AddResourceAsync(
        AddResourceInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Resource = mapper.MapTo(await resourceService.AddAsync(mapper.MapTo(input), false, cancellationToken))
        };

    [UseResolverScope]
    public async Task<ResourcePayload?> UpdateResourceAsync(
        UpdateResourceInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Resource = mapper.MapTo(await resourceService.UpdateAsync(mapper.MapTo(input), cancellationToken))
        };

    [UseResolverScope]
    public async Task<ResourcePayload?> DeleteResourceAsync(
        DeleteResourceInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken) =>
        new() { ClientMutationId = input.ClientMutationId, Resource = mapper.MapTo(await resourceService.DeleteAsync(input.Id, cancellationToken)) };

    [UseResolverScope]
    public async Task<ResourcesPayload?> DeleteResourcesAsync(
        DeleteResourcesInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken)
    {
        var resources = await resourceService.DeleteAsync(input.Ids.RemoveInvalidIds()!.ToList(), cancellationToken);
        return new ResourcesPayload { ClientMutationId = input.ClientMutationId, Resources = resources.Select(mapper.MapTo) };
    }

    [UseResolverScope]
    public async Task<ResourcesPayload?> ActivateResourcesAsync(
        ActivateResourcesInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken)
    {
        var resources = await resourceService.ActivateAsync(input.Ids.RemoveInvalidIds()!.ToList(), cancellationToken);
        return new ResourcesPayload { ClientMutationId = input.ClientMutationId, Resources = resources.Select(mapper.MapTo) };
    }

    [UseResolverScope]
    public async Task<ResourcesPayload?> DeactivateResourcesAsync(
        DeactivateResourcesInput input,
        [Service] IResourceService resourceService,
        CancellationToken cancellationToken)
    {
        var resources = await resourceService.DeactivateAsync(input.Ids.RemoveInvalidIds()!.ToList(), cancellationToken);
        return new ResourcesPayload { ClientMutationId = input.ClientMutationId, Resources = resources.Select(mapper.MapTo) };
    }

    [UseResolverScope]
    public async Task<LocationPayload?> UpdateLocationOpeningHoursAsync(
        UpdateLocationOpeningHoursInput input,
        [Service] ILocationOpeningHoursService locationOpeningHoursService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = mapper.MapTo(
                await locationOpeningHoursService.UpdateOpeningHoursAsync(input.Id, mapper.MapTo(input.WeekOpeningHours)!, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<ResourcePayload?> UpdateLocationResourceAvailableHoursAsync(
        UpdateLocationResourceAvailableHoursInput input,
        [Service] IResourceAvailableHoursService resourceAvailableHoursService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Resource = mapper.MapTo(
                await resourceAvailableHoursService.UpdateAvailableHoursAsync(
                    input.Id,
                    input.OverrideAvailableHours,
                    mapper.MapTo(input.AvailableHours),
                    cancellationToken))
        };
}
