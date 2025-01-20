using Api.Shared.Services.Offering;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL;

[MutationType]
public class Mutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<OrganizationPayload?> AddOrganizationAsync(
        AddOrganizationInput input,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken)
    {
        var organization = await organizationService.AddAsync(mapper.MapTo(input), null, false, cancellationToken);
        return new OrganizationPayload { ClientMutationId = input.ClientMutationId, Organization = mapper.MapTo(organization)! };
    }

    [UseResolverScope]
    public async Task<OrganizationPayload?> UpdateOrganizationAsync(
        UpdateOrganizationInput input,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken)
    {
        var organization = await organizationService.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new OrganizationPayload { ClientMutationId = input.ClientMutationId, Organization = mapper.MapTo(organization)! };
    }

    [UseResolverScope]
    public async Task<OrganizationPayload?> DeleteOrganizationAsync(
        DeleteOrganizationInput input,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken)
    {
        var organization = await organizationService.DeleteAsync(input.Id, cancellationToken);
        return new OrganizationPayload { ClientMutationId = input.ClientMutationId, Organization = mapper.MapTo(organization)! };
    }

    [UseResolverScope]
    public async Task<UpdateOrganizationOfferingPayload?> UpdateOrganizationOfferingAsync(
        UpdateOrganizationOfferingInput input,
        [Service] IOrganizationOfferingService organizationOfferingService,
        CancellationToken cancellationToken)
    {
        await organizationOfferingService.UpdateOfferingAsync(
            input.Id,
            input.OfferingCode.ToOfferingCode(),
            cancellationToken);
        return new UpdateOrganizationOfferingPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<CancelOrganizationOfferingPayload?> CancelOrganizationOfferingAsync(
        CancelOrganizationOfferingInput input,
        [Service] IOrganizationOfferingService organizationOfferingService,
        CancellationToken cancellationToken)
    {
        await organizationOfferingService.CancelOfferingAsync(input.Id, cancellationToken);
        return new CancelOrganizationOfferingPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<OrganizationMemberDetailsPayload?> ChangeOrganizationMemberRoleAsync(
        ChangeOrganizationMemberRoleInput input,
        [Service] IOrganizationMemberService organizationMemberService,
        CancellationToken cancellationToken)
    {
        var organizationMember =
            await organizationMemberService.ChangeRoleAsync(
                input.Id,
                input.Role,
                cancellationToken);
        return new OrganizationMemberDetailsPayload { ClientMutationId = input.ClientMutationId, Member = mapper.MapTo(organizationMember) };
    }

    [UseResolverScope]
    public async Task<OrganizationMembersDetailsPayload?> ChangeOrganizationMembersStatusAsync(
        ChangeOrganizationMembersStatusInput input,
        [Service] IOrganizationMemberService organizationMemberService,
        CancellationToken cancellationToken)
    {
        var organizationMembers =
            await organizationMemberService.ChangeStatusAsync(
                input.Ids,
                input.Status,
                cancellationToken);
        return new OrganizationMembersDetailsPayload
        {
            ClientMutationId = input.ClientMutationId, Members = organizationMembers.Select(mapper.MapTo).ToArray()
        };
    }

    [UseResolverScope]
    public async Task<OrganizationMembersDetailsPayload?> RemoveOrganizationMembersAsync(
        RemoveOrganizationMembersInput input,
        [Service] IOrganizationMemberService organizationMemberService,
        CancellationToken cancellationToken)
    {
        var organizationMembers = await organizationMemberService.RemoveAsync(input.Ids, cancellationToken);
        return new OrganizationMembersDetailsPayload
        {
            ClientMutationId = input.ClientMutationId, Members = organizationMembers.Select(mapper.MapTo).ToArray()
        };
    }

    [UseResolverScope]
    public async Task<InviteCustomersToJoinOrganizationPayload?> InviteCustomersToJoinOrganizationAsync(
        InviteCustomersToJoinOrganizationInput input,
        [Service] IOrganizationInvitationService organizationInvitationService,
        CancellationToken cancellationToken)
    {
        await organizationInvitationService.InviteMembersByEmailsAsync(input.OrganizationId, input.Emails,
            cancellationToken);
        return new InviteCustomersToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<AcceptInvitationToJoinOrganizationPayload?> AcceptInvitationToJoinOrganizationAsync(
        AcceptInvitationToJoinOrganizationInput input,
        [Service] IOrganizationInvitationService organizationInvitationService,
        CancellationToken cancellationToken)
    {
        await organizationInvitationService.AcceptInvitationToJoinAsync(input.Id, cancellationToken);
        return new AcceptInvitationToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<RejectInvitationToJoinOrganizationPayload?> RejectInvitationToJoinOrganizationAsync(
        RejectInvitationToJoinOrganizationInput input,
        [Service] IOrganizationInvitationService organizationInvitationService,
        CancellationToken cancellationToken)
    {
        await organizationInvitationService.RejectInvitationToJoinAsync(input.Id, cancellationToken);
        return new RejectInvitationToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<CancelInvitationToJoinOrganizationPayload?> CancelInvitationToJoinOrganizationAsync(
        CancelInvitationToJoinOrganizationInput input,
        [Service] IOrganizationInvitationService organizationInvitationService,
        CancellationToken cancellationToken)
    {
        await organizationInvitationService.CancelInvitationToJoinAsync(input.Id, cancellationToken);
        return new CancelInvitationToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<OrganizationMemberPayload?> CompleteOrganizationMemberOnboardingAsync(
        CompleteOrganizationMemberOnboardingInput input,
        [Service] IOrganizationMemberService organizationMemberService,
        CancellationToken cancellationToken)
    {
        await organizationMemberService.CompleteOrganizationMemberOnboardingAsync(
            input.OrganizationId,
            cancellationToken);
        return new OrganizationMemberPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<OrganizationTagPayload?> AddCustomTagAsync(
        AddCustomTagInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tag = await tagService.AddAsync(mapper.MapTo(input), false, cancellationToken);
        return new OrganizationTagPayload { ClientMutationId = input.ClientMutationId, OrganizationTag = mapper.MapTo(tag) };
    }

    [UseResolverScope]
    public async Task<OrganizationTagPayload?> UpdateCustomTagAsync(
        UpdateCustomTagInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tag = await tagService.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new OrganizationTagPayload { ClientMutationId = input.ClientMutationId, OrganizationTag = mapper.MapTo(tag) };
    }

    [UseResolverScope]
    public async Task<OrganizationTagPayload?> DeleteCustomTagAsync(
        DeleteCustomTagInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tag = await tagService.DeleteAsync(input.Id, cancellationToken);
        return new OrganizationTagPayload { ClientMutationId = input.ClientMutationId, OrganizationTag = mapper.MapTo(tag) };
    }

    [UseResolverScope]
    public async Task<OrganizationTagsPayload?> DeleteCustomTagsAsync(
        DeleteCustomTagsInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tags = await tagService.DeleteAsync(input.Ids, cancellationToken);
        return new OrganizationTagsPayload { ClientMutationId = input.ClientMutationId, OrganizationTags = tags.Select(mapper.MapTo).ToArray() };
    }

    [UseResolverScope]
    public async Task<OrganizationTagPayload?> AddZoneAsync(
        AddZoneInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tag = await tagService.AddAsync(mapper.MapTo(input), false, cancellationToken);
        return new OrganizationTagPayload { ClientMutationId = input.ClientMutationId, OrganizationTag = mapper.MapTo(tag) };
    }

    [UseResolverScope]
    public async Task<OrganizationTagPayload?> UpdateZoneAsync(
        UpdateZoneInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tag = await tagService.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new OrganizationTagPayload { ClientMutationId = input.ClientMutationId, OrganizationTag = mapper.MapTo(tag) };
    }

    [UseResolverScope]
    public async Task<OrganizationTagPayload?> DeleteZoneAsync(
        DeleteZoneInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tag = await tagService.DeleteAsync(input.Id, cancellationToken);
        return new OrganizationTagPayload { ClientMutationId = input.ClientMutationId, OrganizationTag = mapper.MapTo(tag) };
    }

    [UseResolverScope]
    public async Task<OrganizationTagsPayload?> DeleteZonesAsync(
        DeleteZonesInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tags = await tagService.DeleteAsync(input.Ids, cancellationToken);
        return new OrganizationTagsPayload { ClientMutationId = input.ClientMutationId, OrganizationTags = tags.Select(mapper.MapTo).ToArray() };
    }
}
