using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Mappers;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Organization;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<OrganizationPayload> AddOrganizationAsync(
        AddOrganizationInput input,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization = mapper.MapTo(await organizationService.AddAsync(mapper.MapTo(input), null, false, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationPayload> UpdateOrganizationAsync(
        UpdateOrganizationInput input,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization = mapper.MapTo(await organizationService.UpdateAsync(mapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<OrganizationPayload> DeleteOrganizationAsync(
        DeleteOrganizationInput input,
        [Service] IOrganizationService organizationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Organization = mapper.MapTo(await organizationService.DeleteAsync(input.Id, input.UniqueAlphanumericName, cancellationToken))!
        };
}
