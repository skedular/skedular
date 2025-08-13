using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows.LocationResource;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;

namespace Booking.Shared.Services;

public interface ILocationResourceBookingSlotsHelperService
{
    DateTimeOffset GetStartPeriod();
    ICollection<ResourceBookingSlot> CreateAllAvailableSlots(Resource resource);
    Task GenerateAsync(string locationId, CancellationToken cancellationToken);
    Task GenerateAllAsync(CancellationToken cancellationToken);
}

public class LocationResourceBookingSlotsHelperService(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    ITemporalService temporalService)
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

    public async Task GenerateAsync(string locationId, CancellationToken cancellationToken)
    {
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, false, cancellationToken);
        if (location is null || location.IsReplicatedDeleted() || (location.Organization != null && location.Organization.IsReplicatedDeleted()))
        {
            return;
        }

        await temporalService.StartWorkflowLocationResourceSlotGenerationAsync(
            new LocationResourceSlotGenerationInput(location.Id, null),
            cancellationToken);
    }

    public async Task GenerateAllAsync(CancellationToken cancellationToken)
    {
        var locations = await repositoryFactory.LocationRepository.GetAllWithActiveOrganizationAsync(false, cancellationToken);

        foreach (var location in locations.Where(item => item.Organization == null || item.Organization.IsReplicatedNotDeleted()))
        {
            await temporalService.StartWorkflowLocationResourceSlotGenerationAsync(
                new LocationResourceSlotGenerationInput(location.Id, null),
                cancellationToken);
        }
    }
}
