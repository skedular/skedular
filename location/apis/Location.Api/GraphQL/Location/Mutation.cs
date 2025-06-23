using HotChocolate;
using HotChocolate.Types;
using Location.Api.Mappers;
using Location.Api.Services;

namespace Location.Api.GraphQL.Location;

[MutationType]
public class Mutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<LocationPayload?> AddLocationAsync(
        AddLocationInput input,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = mapper.MapTo(await locationService.AddAsync(mapper.MapTo(input), false, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<LocationPayload?> UpdateLocationAsync(
        UpdateLocationInput input,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = mapper.MapTo(await locationService.UpdateAsync(mapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<LocationPayload?> DeleteLocationAsync(
        DeleteLocationInput input,
        [Service] ILocationService locationService,
        CancellationToken cancellationToken) =>
        new() { ClientMutationId = input.ClientMutationId, Location = mapper.MapTo(await locationService.DeleteAsync(input.Id, cancellationToken))! };

    [UseResolverScope]
    public async Task<LocationPayload?> UpdateLocationOpeningHoursAsync(
        UpdateLocationOpeningHoursInput input,
        [Service] ILocationOpeningHoursService locationOpeningHoursService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Location = mapper.MapTo(
                await locationOpeningHoursService.UpdateOpeningHoursAsync(input.Id, mapper.MapTo(input.WeekOpeningHours)!, cancellationToken))!
        };
}
