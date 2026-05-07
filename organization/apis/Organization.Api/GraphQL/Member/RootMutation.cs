using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Member;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<OrganizationMemberDetailsPayload> ChangeOrganizationMemberRoleAsync(
        ChangeOrganizationMemberRoleInput input,
        [Service] IOrganizationMemberService organizationMemberService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Member = graphQlMapper.MapTo(await organizationMemberService.ChangeRoleAsync(input.Id, input.Role, cancellationToken))
        };

    [UseResolverScope]
    public async Task<OrganizationMembersDetailsPayload> ChangeOrganizationMembersStatusAsync(
        ChangeOrganizationMembersStatusInput input,
        [Service] IOrganizationMemberService organizationMemberService,
        CancellationToken cancellationToken)
    {
        var organizationMembers =
            await organizationMemberService.ChangeStatusAsync(input.Ids.RemoveInvalidIds().ToList(), input.Status, cancellationToken);
        return new OrganizationMembersDetailsPayload
        {
            ClientMutationId = input.ClientMutationId, Members = organizationMembers.Select(graphQlMapper.MapTo).ToArray()
        };
    }

    [UseResolverScope]
    public async Task<OrganizationMembersDetailsPayload> RemoveOrganizationMembersAsync(
        RemoveOrganizationMembersInput input,
        [Service] IOrganizationMemberService organizationMemberService,
        CancellationToken cancellationToken)
    {
        var organizationMembers = await organizationMemberService.RemoveAsync(input.Ids.RemoveInvalidIds().ToList(), cancellationToken);
        return new OrganizationMembersDetailsPayload
        {
            ClientMutationId = input.ClientMutationId, Members = organizationMembers.Select(graphQlMapper.MapTo).ToArray()
        };
    }

    [UseResolverScope]
    public async Task<OrganizationMemberPayload> CompleteOrganizationMemberOnboardingAsync(
        CompleteOrganizationMemberOnboardingInput input,
        [Service] IOrganizationMemberService organizationMemberService,
        CancellationToken cancellationToken)
    {
        await organizationMemberService.CompleteOrganizationMemberOnboardingAsync(
            input.OrganizationId,
            input.OrganizationCustomDomain,
            cancellationToken);
        return new OrganizationMemberPayload { ClientMutationId = input.ClientMutationId };
    }
}
