using HotChocolate;
using HotChocolate.Types;
using Location.Api.GraphQL.Location;
using Location.Api.Mappers;
using Location.Api.Services;

namespace Location.Api.GraphQL.ContactedVia;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<LocationPayload> ToggleContactedViaEmailAsync(
        ToggleContactedViaEmailInput input,
        [Service]
        ILocationContactedViaService locationContactedViaService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = graphQlMapper.MapTo(await locationContactedViaService.ToggleContactedViaEmailAsync(input.LocationId, cancellationToken))!,
        };

    [UseResolverScope]
    public async Task<LocationPayload> ToggleContactedViaCallAsync(
        ToggleContactedViaCallInput input,
        [Service]
        ILocationContactedViaService locationContactedViaService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = graphQlMapper.MapTo(await locationContactedViaService.ToggleContactedViaCallAsync(input.LocationId, cancellationToken))!,
        };

    [UseResolverScope]
    public async Task<LocationPayload> ToggleContactedViaSmsAsync(
        ToggleContactedViaSmsInput input,
        [Service]
        ILocationContactedViaService locationContactedViaService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = graphQlMapper.MapTo(await locationContactedViaService.ToggleContactedViaSmsAsync(input.LocationId, cancellationToken))!,
        };

    [UseResolverScope]
    public async Task<LocationPayload> ToggleContactedViaWhatsappAsync(
        ToggleContactedViaWhatsappInput input,
        [Service]
        ILocationContactedViaService locationContactedViaService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location =
                graphQlMapper.MapTo(await locationContactedViaService.ToggleContactedViaWhatsappAsync(input.LocationId, cancellationToken))!,
        };
}
