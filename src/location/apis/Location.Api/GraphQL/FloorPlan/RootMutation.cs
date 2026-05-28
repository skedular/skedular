using HotChocolate;
using HotChocolate.Types;
using Location.Api.Mappers;
using Location.Api.Models;
using Location.Api.Services;

namespace Location.Api.GraphQL.FloorPlan;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
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
                graphQlMapper.MapTo(await floorPlanService.AddAsync(graphQlMapper.MapTo(input), input.ResourcePositions is not null,
                    cancellationToken))!
        };

    [UseResolverScope]
    public async Task<FloorPlanPayload> UpdateFloorPlanAsync(
        UpdateFloorPlanInput input,
        [Service] IFloorPlanService floorPlanService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            FloorPlan = graphQlMapper.MapTo(
                await floorPlanService.UpdateAsync(
                    new FloorPlanPatchRequest(graphQlMapper.MapTo(input), input.FieldsToUpdate),
                    cancellationToken))!
        };

    [UseResolverScope]
    public async Task<FloorPlanPayload> DeleteFloorPlanAsync(
        DeleteFloorPlanInput input,
        [Service] IFloorPlanService floorPlanService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            FloorPlan = graphQlMapper.MapTo(await floorPlanService.DeleteAsync(input.Id, cancellationToken))!
        };

    [UseResolverScope]
    public async Task<FloorPlanPayload> UpdateResourcePositionsAsync(
        UpdateResourcePositionsInput input,
        [Service] IFloorPlanService floorPlanService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            FloorPlan = graphQlMapper.MapTo(
                await floorPlanService.UpdateResourcePositionsAsync(
                    new ResourcePositionsPatchRequest(input.FloorPlanId, graphQlMapper.MapTo(input).ToList(), input.FieldsToUpdate),
                    cancellationToken))!
        };
}
