using Api.Shared.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Context;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL;

public class OrganizationMutation(IServiceProvider serviceProvider, IMapper mapper)
{
    public async Task<OrganizationPayload?> AddOrganizationAsync(
        AddOrganizationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationService>();
        var organization = await service.AddAsync(mapper.MapTo(input), null, false, cancellationToken);
        return new OrganizationPayload
        {
            ClientMutationId = input.ClientMutationId, Organization = mapper.MapTo(organization)!
        };
    }

    public async Task<OrganizationPayload?> UpdateOrganizationAsync(
        UpdateOrganizationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationService>();
        var organization = await service.UpdateAsync(mapper.MapTo(input), cancellationToken);
        return new OrganizationPayload
        {
            ClientMutationId = input.ClientMutationId, Organization = mapper.MapTo(organization)!
        };
    }

    public async Task<OrganizationPayload?> DeleteOrganizationAsync(
        DeleteOrganizationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationService>();
        var organization = await service.DeleteAsync(input.Id, cancellationToken);
        return new OrganizationPayload
        {
            ClientMutationId = input.ClientMutationId, Organization = mapper.MapTo(organization)!
        };
    }

    public async Task<UpdateOrganizationOfferingPayload?> UpdateOrganizationOfferingAsync(
        UpdateOrganizationOfferingInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationOfferingService>();
        await service.UpdateOfferingAsync(
            input.Id,
            input.OfferingCode.ToOfferingCode(),
            cancellationToken);
        return new UpdateOrganizationOfferingPayload { ClientMutationId = input.ClientMutationId };
    }

    public async Task<CancelOrganizationOfferingPayload?> CancelOrganizationOfferingAsync(
        CancelOrganizationOfferingInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationOfferingService>();
        await service.CancelOfferingAsync(input.Id, cancellationToken);
        return new CancelOrganizationOfferingPayload { ClientMutationId = input.ClientMutationId };
    }

    public async Task<OrganizationMemberDetailsPayload?> ChangeOrganizationMemberOwnershipTypeAsync(
        ChangeOrganizationMemberOwnershipTypeInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationMemberService>();
        var organizationMember =
            await service.ChangeMembershipTypeAsync(
                input.Id,
                input.MembershipType switch
                {
                    OrganizationMemberMembershipType.OWNER => OrganizationMembershipType.Owner,
                    OrganizationMemberMembershipType.ADMINISTRATOR => OrganizationMembershipType.Administrator,
                    OrganizationMemberMembershipType.MEMBER => OrganizationMembershipType.Member,
                    _ => throw new ArgumentOutOfRangeException()
                },
                cancellationToken);
        return new OrganizationMemberDetailsPayload
        {
            ClientMutationId = input.ClientMutationId, Member = mapper.MapTo(organizationMember)
        };
    }

    public async Task<InviteCustomersToJoinOrganizationPayload?> InviteCustomersToJoinOrganizationAsync(
        InviteCustomersToJoinOrganizationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationInvitationService>();
        await service.InviteMembersByEmailsAsync(input.OrganizationId, input.Emails, cancellationToken);
        return new InviteCustomersToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }

    public async Task<AcceptInvitationToJoinOrganizationPayload?> AcceptInvitationToJoinOrganizationAsync(
        AcceptInvitationToJoinOrganizationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationInvitationService>();
        await service.AcceptInvitationToJoinAsync(input.Id, cancellationToken);
        return new AcceptInvitationToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }

    public async Task<RejectInvitationToJoinOrganizationPayload?> RejectInvitationToJoinOrganizationAsync(
        RejectInvitationToJoinOrganizationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationInvitationService>();
        await service.RejectInvitationToJoinAsync(input.Id, cancellationToken);
        return new RejectInvitationToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }

    public async Task<CancelInvitationToJoinOrganizationPayload?> CancelInvitationToJoinOrganizationAsync(
        CancelInvitationToJoinOrganizationInput input,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationInvitationService>();
        await service.CancelInvitationToJoinAsync(input.Id, cancellationToken);
        return new CancelInvitationToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }
}
