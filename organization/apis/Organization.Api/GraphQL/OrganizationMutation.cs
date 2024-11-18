using Api.Shared.Models;
using Api.Shared.Services.Offering;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL;

public class OrganizationMutation
{
    [UseServiceScope]
    public async Task<OrganizationPayload?> AddOrganizationAsync(
        AddOrganizationInput input,
        [Service] IOrganizationService organizationService,
        [Service] IMapper mapper,
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
        [Service] IMapper mapper,
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
        [Service] IMapper mapper,
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
        [Service] IMapper mapper,
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
}
