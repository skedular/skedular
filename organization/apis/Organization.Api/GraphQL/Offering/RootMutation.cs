using Api.Shared.Services.Offering;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Offering;

[MutationType]
public class RootMutation
{
    [UseResolverScope]
    public async Task<UpdateOrganizationOfferingPayload> UpdateOrganizationOfferingAsync(
        UpdateOrganizationOfferingInput input,
        [Service] IOrganizationOfferingService organizationOfferingService,
        CancellationToken cancellationToken)
    {
        await organizationOfferingService.UpdateOfferingAsync(
            input.OrganizationId,
            input.OrganizationUniqueAlphanumericName,
            input.OfferingCode.ToOfferingCode(),
            false,
            cancellationToken);
        return new UpdateOrganizationOfferingPayload { ClientMutationId = input.ClientMutationId };
    }

    [UseResolverScope]
    public async Task<CancelOrganizationOfferingPayload> CancelOrganizationOfferingAsync(
        CancelOrganizationOfferingInput input,
        [Service] IOrganizationOfferingService organizationOfferingService,
        CancellationToken cancellationToken)
    {
        await organizationOfferingService.CancelOfferingAsync(input.OrganizationId, input.OrganizationUniqueAlphanumericName, cancellationToken);
        return new CancelOrganizationOfferingPayload { ClientMutationId = input.ClientMutationId };
    }
}
