using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Sso;

[MutationType]
public class Mutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<UpdateOrganizationSsoSettingsPayload?> UpdateOrganizationSsoSettingsAsync(
        UpdateOrganizationSsoSettingsInput input,
        [Service] IOrganizationSsoService organizationSsoService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization = mapper.MapTo(await organizationSsoService.UpdateSsoSettingsAsync(mapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<UpdateOrganizationSsoSettingsPayload?> RemoveOrganizationSsoSettingsAsync(
        RemoveOrganizationSsoSettingsInput input,
        [Service] IOrganizationSsoService organizationSsoService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization = mapper.MapTo(await organizationSsoService.RemoveSsoSettingsAsync(input.OrganizationId, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<UpdateOrganizationSsoSettingsPayload?> ToggleOrganizationSsoAsync(
        ToggleOrganizationSsoInput input,
        [Service] IOrganizationSsoService organizationSsoService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization = mapper.MapTo(
                await organizationSsoService.ToggleSsoSettingsAsync(input.OrganizationId, input.IsActive, cancellationToken))!
        };
}
