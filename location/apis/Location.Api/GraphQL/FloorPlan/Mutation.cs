using HotChocolate;
using HotChocolate.Types;
using Location.Api.Mappers;
using Location.Api.Services;

namespace Location.Api.GraphQL.FloorPlan;

[MutationType]
public class Mutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<FloorPlanPayload> AddFloorPlanAsync(
        AddFloorPlanInput input,
        [Service] IFloorPlanService floorPlanService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            FloorPlan =
                mapper.MapTo(await floorPlanService.AddAsync(mapper.MapTo(input), input.ResourcePositions is not null, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<FloorPlanPayload> UpdateFloorPlanAsync(
        UpdateFloorPlanInput input,
        [Service] IFloorPlanService floorPlanService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            FloorPlan = mapper.MapTo(await floorPlanService.UpdateAsync(mapper.MapTo(input), input.ResourcePositions is not null,
                cancellationToken))!
        };

    [UseResolverScope]
    public async Task<FloorPlanPayload> DeleteFloorPlanAsync(
        DeleteFloorPlanInput input,
        [Service] IFloorPlanService floorPlanService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId, FloorPlan = mapper.MapTo(await floorPlanService.DeleteAsync(input.Id, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<FloorPlanPayload> UpdateResourcePositionsAsync(
        UpdateResourcePositionsInput input,
        [Service] IFloorPlanService floorPlanService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            FloorPlan = mapper.MapTo(
                await floorPlanService.UpdateResourcePositionsAsync(input.FloorPlanId, mapper.MapTo(input).ToList(), cancellationToken))!
        };
}
