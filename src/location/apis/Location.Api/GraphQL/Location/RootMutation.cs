using HotChocolate;
using HotChocolate.Types;
using Location.Api.Mappers;
using Location.Api.Models;
using Location.Api.Services;

namespace Location.Api.GraphQL.Location;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<LocationPayload> AddLocationAsync(
        AddLocationInput input,
        [Service]
        ILocationService locationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = graphQlMapper.MapTo(await locationService.AddAsync(graphQlMapper.MapTo(input), false, cancellationToken))!,
        };

    [UseResolverScope]
    public async Task<LocationPayload> UpdateLocationAsync(
        UpdateLocationInput input,
        [Service]
        ILocationService locationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = graphQlMapper.MapTo(
                await locationService.UpdateAsync(
                    new LocationPatchRequest(graphQlMapper.MapTo(input), input.FieldsToUpdate),
                    false,
                    cancellationToken))!,
        };

    [UseResolverScope]
    public async Task<LocationPayload> DeleteLocationAsync(
        DeleteLocationInput input,
        [Service]
        ILocationService locationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = graphQlMapper.MapTo(await locationService.DeleteAsync(input.Id, cancellationToken))!,
        };

    [UseResolverScope]
    public async Task<LocationPayload> UpdateLocationOpeningHoursAsync(
        UpdateLocationOpeningHoursInput input,
        [Service]
        ILocationOpeningHoursService locationOpeningHoursService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = graphQlMapper.MapTo(
                await locationOpeningHoursService.UpdateOpeningHoursAsync(
                    new LocationOpeningHoursPatchRequest(input.Id, graphQlMapper.MapTo(input.WeekOpeningHours)!, input.FieldsToUpdate),
                    cancellationToken))!,
        };

    [UseResolverScope]
    public async Task<LocationPayload> AddLocationRestrictedInformationAsync(
        AddLocationRestrictedInformationInput input,
        [Service]
        ILocationRestrictedInformationService locationRestrictedInformationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = graphQlMapper.MapTo(await locationRestrictedInformationService.AddAsync(graphQlMapper.MapTo(input), cancellationToken))!,
        };

    [UseResolverScope]
    public async Task<LocationPayload> UpdateLocationRestrictedInformationAsync(
        UpdateLocationRestrictedInformationInput input,
        [Service]
        ILocationRestrictedInformationService locationRestrictedInformationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = graphQlMapper.MapTo(
                await locationRestrictedInformationService.UpdateAsync(
                    new LocationRestrictedInformationPatchRequest(graphQlMapper.MapTo(input), input.FieldsToUpdate),
                    cancellationToken))!,
        };

    [UseResolverScope]
    public async Task<LocationPayload> DeleteLocationRestrictedInformationAsync(
        DeleteLocationRestrictedInformationInput input,
        [Service]
        ILocationRestrictedInformationService locationRestrictedInformationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = graphQlMapper.MapTo(await locationRestrictedInformationService.DeleteAsync(input.Id, cancellationToken))!,
        };
}
