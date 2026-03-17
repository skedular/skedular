using HotChocolate;
using HotChocolate.Types;
using Organization.Api.GraphQL.Organization;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Sso;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<OrganizationPayload> UpdateOrganizationSsoSettingsAsync(
        UpdateOrganizationSsoSettingsInput input,
        [Service] IOrganizationSsoService organizationSsoService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization = mapper.MapTo(await organizationSsoService.UpdateAsync(mapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationPayload> RemoveOrganizationSsoSettingsAsync(
        RemoveOrganizationSsoSettingsInput input,
        [Service] IOrganizationSsoService organizationSsoService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization = mapper.MapTo(
                await organizationSsoService.RemoveAsync(input.OrganizationId, input.OrganizationCustomDomain, cancellationToken))!
        };
}
