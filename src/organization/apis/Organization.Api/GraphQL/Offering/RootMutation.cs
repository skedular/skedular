using Api.Shared.Services.Offering;
using HotChocolate;
using HotChocolate.Types;
using Organization.Api.Models;
using Organization.Api.Services;

namespace Organization.Api.GraphQL.Offering;

[MutationType]
public class RootMutation
{
    [UseResolverScope]
    public async Task<UpdateOrganizationOfferingPayload> UpdateOrganizationOfferingAsync(
        UpdateOrganizationOfferingInput input,
        [Service]
        IOrganizationOfferingService organizationOfferingService,
        CancellationToken cancellationToken)
    {
        await organizationOfferingService.UpdateOfferingPatchAsync(
            new OrganizationOfferingPatchRequest(
                input.OrganizationId,
                input.OrganizationCustomDomain,
                input.FieldsToUpdate,
                string.IsNullOrWhiteSpace(input.OfferingCode) ? null : input.OfferingCode.ToOfferingCode()),
            cancellationToken);
        return new UpdateOrganizationOfferingPayload
        {
            ClientMutationId = input.ClientMutationId,
        };
    }

    [UseResolverScope]
    public async Task<CancelOrganizationOfferingPayload> CancelOrganizationOfferingAsync(
        CancelOrganizationOfferingInput input,
        [Service]
        IOrganizationOfferingService organizationOfferingService,
        CancellationToken cancellationToken)
    {
        await organizationOfferingService.CancelOfferingAsync(input.OrganizationId, input.OrganizationCustomDomain, cancellationToken);
        return new CancelOrganizationOfferingPayload
        {
            ClientMutationId = input.ClientMutationId,
        };
    }
}
