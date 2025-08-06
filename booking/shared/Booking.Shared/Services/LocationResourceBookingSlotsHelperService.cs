using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows.LocationResource;
using Enterprise.Shared.Random;
using Enterprise.Shared.Temporal.Configurations;
using Enterprise.Shared.Time;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Booking.Shared.Services;

public interface ILocationResourceBookingSlotsHelperService
{
    DateTimeOffset GetStartPeriod();
    ICollection<ResourceBookingSlot> CreateAllAvailableSlots(Resource resource);
    Task GenerateAsync(string locationId, CancellationToken cancellationToken);
    Task GenerateAllAsync(CancellationToken cancellationToken);
}

public class LocationResourceBookingSlotsHelperService(
    TemporalConfiguration temporalConfiguration,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ITemporalClient temporalClient,
    TimeProvider timeProvider)
    : ILocationResourceBookingSlotsHelperService
{
    public DateTimeOffset GetStartPeriod() => timeProvider.GetUtcNow().StartOfDay().AddDays(-14);

    public ICollection<ResourceBookingSlot> CreateAllAvailableSlots(Resource resource)
    {
        var startPeriod = GetStartPeriod();
        var endPeriod = startPeriod.AddDays(14).AddYears(1);
        var count = (endPeriod - startPeriod).TotalMinutes / OpeningHoursDetails.OpeningHoursSlotSizeInMinutes;

        return Enumerable
            .Range(0, (int)count)
            .Select(idx => startPeriod.AddMinutes(idx * OpeningHoursDetails.OpeningHoursSlotSizeInMinutes))
            .Select(start => new ResourceBookingSlot { Id = randomHelper.Generate(), Start = start, Available = true, Resource = resource })
            .ToList();
    }

    public async Task GenerateAsync(string locationId, CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync(
            (LocationResourceSlotGeneration workflow) =>
                workflow.ExecuteAsync(new LocationResourceSlotGenerationInput(locationId, null)),
            new WorkflowOptions
            {
                Id = randomHelper.Generate(),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.RejectDuplicate,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });

    public async Task GenerateAllAsync(CancellationToken cancellationToken)
    {
        var locations = await repositoryFactory.LocationRepository.GetAllAsync(false, cancellationToken);

        foreach (var locationId in locations.Select(item => item.Id))
        {
            await temporalClient.StartWorkflowAsync(
                (LocationResourceSlotGeneration workflow) =>
                    workflow.ExecuteAsync(new LocationResourceSlotGenerationInput(locationId, null)),
                new WorkflowOptions
                {
                    Id = randomHelper.Generate(),
                    TaskQueue = temporalConfiguration.Worker.TaskQueue,
                    RetryPolicy = null,
                    IdReusePolicy = WorkflowIdReusePolicy.RejectDuplicate,
                    Rpc = new RpcOptions { CancellationToken = cancellationToken }
                });
        }
    }
}
