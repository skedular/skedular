using Api.Shared.Models;
using Api.Shared.Services.Offering;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL;

public class OrganizationMutation(IMapper mapper)
{
    [UseServiceScope]
    public async Task<OrganizationPayload?> AddOrganizationAsync(
        AddOrganizationInput input,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken)
    {
        var organization = await organizationService.AddAsync(mapper.MapTo(input), null, false, cancellationToken);
        return new OrganizationPayload
        {
            ClientMutationId = input.ClientMutationId, Organization = mapper.MapTo(organization)!
        };
    }

    [UseServiceScope]
    public async Task<OrganizationPayload?> UpdateOrganizationAsync(
        UpdateOrganizationInput input,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken)
    {
        var organization = await organizationService.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new OrganizationPayload
        {
            ClientMutationId = input.ClientMutationId, Organization = mapper.MapTo(organization)!
        };
    }

    [UseServiceScope]
    public async Task<OrganizationPayload?> DeleteOrganizationAsync(
        DeleteOrganizationInput input,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken)
    {
        var organization = await organizationService.DeleteAsync(input.Id, cancellationToken);
        return new OrganizationPayload
        {
            ClientMutationId = input.ClientMutationId, Organization = mapper.MapTo(organization)!
        };
    }

    [UseServiceScope]
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

    [UseServiceScope]
    public async Task<CancelOrganizationOfferingPayload?> CancelOrganizationOfferingAsync(
        CancelOrganizationOfferingInput input,
        [Service] IOrganizationOfferingService organizationOfferingService,
        CancellationToken cancellationToken)
    {
        await organizationOfferingService.CancelOfferingAsync(input.Id, cancellationToken);
        return new CancelOrganizationOfferingPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseServiceScope]
    public async Task<OrganizationMemberDetailsPayload?> ChangeOrganizationMemberOwnershipTypeAsync(
        ChangeOrganizationMemberOwnershipTypeInput input,
        [Service] IOrganizationMemberService organizationMemberService,
        CancellationToken cancellationToken)
    {
        var organizationMember =
            await organizationMemberService.ChangeMembershipTypeAsync(
                input.Id,
                input.MembershipType switch
                {
                    OrganizationMemberMembershipType.Owner => OrganizationMembershipType.Owner,
                    OrganizationMemberMembershipType.Administrator => OrganizationMembershipType.Administrator,
                    OrganizationMemberMembershipType.Member => OrganizationMembershipType.Member,
                    _ => throw new ArgumentOutOfRangeException()
                },
                cancellationToken);
        return new OrganizationMemberDetailsPayload
        {
            ClientMutationId = input.ClientMutationId, Member = mapper.MapTo(organizationMember)
        };
    }

    [UseServiceScope]
    public async Task<InviteCustomersToJoinOrganizationPayload?> InviteCustomersToJoinOrganizationAsync(
        InviteCustomersToJoinOrganizationInput input,
        [Service] IOrganizationInvitationService organizationInvitationService,
        CancellationToken cancellationToken)
    {
        await organizationInvitationService.InviteMembersByEmailsAsync(input.OrganizationId, input.Emails,
            cancellationToken);
        return new InviteCustomersToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseServiceScope]
    public async Task<AcceptInvitationToJoinOrganizationPayload?> AcceptInvitationToJoinOrganizationAsync(
        AcceptInvitationToJoinOrganizationInput input,
        [Service] IOrganizationInvitationService organizationInvitationService,
        CancellationToken cancellationToken)
    {
        await organizationInvitationService.AcceptInvitationToJoinAsync(input.Id, cancellationToken);
        return new AcceptInvitationToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseServiceScope]
    public async Task<RejectInvitationToJoinOrganizationPayload?> RejectInvitationToJoinOrganizationAsync(
        RejectInvitationToJoinOrganizationInput input,
        [Service] IOrganizationInvitationService organizationInvitationService,
        CancellationToken cancellationToken)
    {
        await organizationInvitationService.RejectInvitationToJoinAsync(input.Id, cancellationToken);
        return new RejectInvitationToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseServiceScope]
    public async Task<CancelInvitationToJoinOrganizationPayload?> CancelInvitationToJoinOrganizationAsync(
        CancelInvitationToJoinOrganizationInput input,
        [Service] IOrganizationInvitationService organizationInvitationService,
        CancellationToken cancellationToken)
    {
        await organizationInvitationService.CancelInvitationToJoinAsync(input.Id, cancellationToken);
        return new CancelInvitationToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseServiceScope]
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

    [UseServiceScope]
    public async Task<OrganizationTagPayload?> AddDeskTypeAsync(
        AddDeskTypeInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tag = await tagService.AddAsync(mapper.MapTo(input), false, cancellationToken);
        return new OrganizationTagPayload
        {
            ClientMutationId = input.ClientMutationId, OrganizationTag = mapper.MapTo(tag)
        };
    }

    [UseServiceScope]
    public async Task<OrganizationTagPayload?> UpdateDeskTypeAsync(
        UpdateDeskTypeInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tag = await tagService.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new OrganizationTagPayload
        {
            ClientMutationId = input.ClientMutationId, OrganizationTag = mapper.MapTo(tag)
        };
    }

    [UseServiceScope]
    public async Task<OrganizationTagPayload?> DeleteDeskTypeAsync(
        DeleteDeskTypeInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tag = await tagService.DeleteAsync(input.Id, cancellationToken);
        return new OrganizationTagPayload
        {
            ClientMutationId = input.ClientMutationId, OrganizationTag = mapper.MapTo(tag)
        };
    }

    [UseServiceScope]
    public async Task<OrganizationTagPayload?> AddZoneAsync(
        AddZoneInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tag = await tagService.AddAsync(mapper.MapTo(input), false, cancellationToken);
        return new OrganizationTagPayload
        {
            ClientMutationId = input.ClientMutationId, OrganizationTag = mapper.MapTo(tag)
        };
    }

    [UseServiceScope]
    public async Task<OrganizationTagPayload?> UpdateZoneAsync(
        UpdateZoneInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tag = await tagService.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new OrganizationTagPayload
        {
            ClientMutationId = input.ClientMutationId, OrganizationTag = mapper.MapTo(tag)
        };
    }

    [UseServiceScope]
    public async Task<OrganizationTagPayload?> DeleteZoneAsync(
        DeleteZoneInput input,
        [Service] ITagService tagService,
        CancellationToken cancellationToken)
    {
        var tag = await tagService.DeleteAsync(input.Id, cancellationToken);
        return new OrganizationTagPayload
        {
            ClientMutationId = input.ClientMutationId, OrganizationTag = mapper.MapTo(tag)
        };
    }
}
