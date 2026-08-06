using HotChocolate;
using HotChocolate.Types;
using Organization.Api.GraphQL.Organization;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Xero;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<OrganizationPayload> UpdateOrganizationXeroConnectionAsync(
        UpdateOrganizationXeroConnectionInput input,
        [Service]
        IOrganizationXeroConnectionService organizationXeroConnectionService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization =
                graphQlMapper.MapTo(await organizationXeroConnectionService.UpdatePatchAsync(graphQlMapper.MapTo(input), cancellationToken))!,
        };

    [UseResolverScope]
    public async Task<OrganizationPayload> DisconnectOrganizationXeroConnectionAsync(
        DisconnectOrganizationXeroConnectionInput input,
        [Service]
        IOrganizationXeroConnectionService organizationXeroConnectionService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization = graphQlMapper.MapTo(await organizationXeroConnectionService.RemoveAsync(
                input.OrganizationId,
                input.OrganizationCustomDomain,
                cancellationToken))!,
        };
}
