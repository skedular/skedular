using HotChocolate;
using HotChocolate.Types;
using Organization.Api.GraphQL.Organization;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Xero;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<OrganizationPayload> UpdateOrganizationXeroConnectionAsync(
        UpdateOrganizationXeroConnectionInput input,
        [Service] IOrganizationXeroConnectionService organizationXeroConnectionService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization = mapper.MapTo(await organizationXeroConnectionService.UpdateAsync(mapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationPayload> DisconnectOrganizationXeroConnectionAsync(
        DisconnectOrganizationXeroConnectionInput input,
        [Service] IOrganizationXeroConnectionService organizationXeroConnectionService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization = mapper.MapTo(await organizationXeroConnectionService.RemoveAsync(
                input.OrganizationId,
                input.OrganizationCustomDomain,
                cancellationToken))!
        };
}
