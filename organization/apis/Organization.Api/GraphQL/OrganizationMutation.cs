using Api.Shared.Models;
using Api.Shared.Services.GraphQL.UnityHub.V1.Organization;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Context;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL;

public class OrganizationMutation(IMapper mapper) : Mutation
{
    public override async Task<OrganizationPayload?> AddOrganizationAsync(
        AddOrganizationInput input,
        IServiceProvider serviceProvider,
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

    public override async Task<OrganizationPayload?> UpdateOrganizationAsync(
        UpdateOrganizationInput input,
        IServiceProvider serviceProvider,
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

    public override async Task<OrganizationPayload?> DeleteOrganizationAsync(
        DeleteOrganizationInput input,
        IServiceProvider serviceProvider,
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

    public override async Task<UpdateOrganizationOfferingPayload?> UpdateOrganizationOfferingAsync(
        UpdateOrganizationOfferingInput input,
        IServiceProvider serviceProvider,
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

    public override async Task<CancelOrganizationOfferingPayload?> CancelOrganizationOfferingAsync(
        CancelOrganizationOfferingInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationOfferingService>();
        await service.CancelOfferingAsync(input.Id, cancellationToken);
        return new CancelOrganizationOfferingPayload { ClientMutationId = input.ClientMutationId };
    }

    public override async Task<OrganizationMemberDetailsPayload?> ChangeOrganizationMemberOwnershipTypeAsync(
        ChangeOrganizationMemberOwnershipTypeInput input,
        IServiceProvider serviceProvider,
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

    public override async Task<InviteCustomersToJoinOrganizationPayload?> InviteCustomersToJoinOrganizationAsync(
        InviteCustomersToJoinOrganizationInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationInvitationService>();
        await service.InviteMembersByEmailsAsync(input.OrganizationId, input.Emails, cancellationToken);
        return new InviteCustomersToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }

    public override async Task<AcceptInvitationToJoinOrganizationPayload?> AcceptInvitationToJoinOrganizationAsync(
        AcceptInvitationToJoinOrganizationInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationInvitationService>();
        await service.AcceptInvitationToJoinAsync(input.Id, cancellationToken);
        return new AcceptInvitationToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }

    public override async Task<RejectInvitationToJoinOrganizationPayload?> RejectInvitationToJoinOrganizationAsync(
        RejectInvitationToJoinOrganizationInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationInvitationService>();
        await service.RejectInvitationToJoinAsync(input.Id, cancellationToken);
        return new RejectInvitationToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }

    public override async Task<CancelInvitationToJoinOrganizationPayload?> CancelInvitationToJoinOrganizationAsync(
        CancelInvitationToJoinOrganizationInput input,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateScopeAndSetContent();
        var service = scope.ServiceProvider.GetRequiredService<IOrganizationInvitationService>();
        await service.CancelInvitationToJoinAsync(input.Id, cancellationToken);
        return new CancelInvitationToJoinOrganizationPayload { ClientMutationId = input.ClientMutationId };
    }
}
