using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Organization;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<OrganizationPayload> AddOrganizationAsync(
        AddOrganizationInput input,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization = graphQlMapper.MapTo(await organizationService.AddAsync(graphQlMapper.MapTo(input), null, false, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationPayload> UpdateOrganizationAsync(
        UpdateOrganizationInput input,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken)
    {
        var result = await organizationService.UpdatePatchAsync(graphQlMapper.MapTo(input), cancellationToken);
        return new OrganizationPayload { ClientMutationId = input.ClientMutationId, Organization = graphQlMapper.MapTo(result.Organization)! };
    }

    [UseResolverScope]
    public async Task<OrganizationPayload> DeleteOrganizationAsync(
        DeleteOrganizationInput input,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization = graphQlMapper.MapTo(await organizationService.DeleteAsync(input.Id, input.CustomDomain, cancellationToken))!
        };
}
