using HotChocolate;
using HotChocolate.Types;
using Location.Api.Mappers;
using Location.Api.Services;

namespace Location.Api.GraphQL.Location;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<LocationPayload> AddLocationAsync(
        AddLocationInput input,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = graphQlMapper.MapTo(await locationService.AddAsync(graphQlMapper.MapTo(input), false, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<LocationPayload> UpdateLocationAsync(
        UpdateLocationInput input,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = graphQlMapper.MapTo(await locationService.UpdateAsync(graphQlMapper.MapTo(input), false, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<LocationPayload> DeleteLocationAsync(
        DeleteLocationInput input,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = graphQlMapper.MapTo(await locationService.DeleteAsync(input.Id, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<LocationPayload> UpdateLocationOpeningHoursAsync(
        UpdateLocationOpeningHoursInput input,
        [Service] ILocationOpeningHoursService locationOpeningHoursService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = graphQlMapper.MapTo(
                await locationOpeningHoursService.UpdateOpeningHoursAsync(input.Id, graphQlMapper.MapTo(input.WeekOpeningHours)!, cancellationToken))!
        };
}
