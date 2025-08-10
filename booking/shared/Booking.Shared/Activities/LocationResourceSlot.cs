using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows.LocationResource;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Enterprise.Shared.Temporal.Configurations;
using Enterprise.Shared.Time;
using Temporalio.Activities;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Booking.Shared.Activities;

public class LocationResourceSlot(
    TemporalConfiguration temporalConfiguration,
    IRepositoryFactory repositoryFactory,
    ILocationResourceBookingSlotsHelperService locationResourceBookingSlotsHelperService,
    IRandomHelper randomHelper,
    ITemporalClient temporalClient,
    TimeProvider timeProvider)
{
    [Activity]
    public async Task<ExecuteAllLocationResourcesSlotGenerationWorkflowsResponse> ExecuteAllLocationResourcesSlotGenerationWorkflowsAsync(
        string locationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, false, cancellationToken);

        return location is null || location.IsReplicatedDeleted() || (location.Organization != null && location.Organization.IsReplicatedDeleted())
            ? new ExecuteAllLocationResourcesSlotGenerationWorkflowsResponse(false, [])
            : new ExecuteAllLocationResourcesSlotGenerationWorkflowsResponse(true, location.Resources.Select(item => item.Id).ToList());
    }

    [Activity]
    public async Task GenerateMissingResourceSlotsAsync(string resourceId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;

        var resource = await repositoryFactory.ResourceRepository.GetByIdAsync(resourceId, false, cancellationToken);
        if (resource is null)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(resource.Location);

        var openingHours = (resource.IsAvailableHoursOverridden.HasValue && resource.IsAvailableHoursOverridden.Value
            ? resource.AvailableHours
            : resource.Location.OpeningHours) ?? OpeningHours.Default;

        var existingResourceBookingSlots = await repositoryFactory.ResourceBookingSlotRepository.GetByResourceIdAsync(
            resourceId,
            locationResourceBookingSlotsHelperService.GetStartPeriod(),
            cancellationToken);

        var savingNeeded = false;
        var allSlots = locationResourceBookingSlotsHelperService.CreateAllAvailableSlots(resource);
        var allSlotsDictionary = allSlots.ToDictionary(item => item.Start, item => item);
        var existingResourceBookingSlotsDictionary = existingResourceBookingSlots.ToDictionary(item => item.Start, item => item);
        var slotsToAdd = allSlots.Where(item => !existingResourceBookingSlotsDictionary.ContainsKey(item.Start)).ToList();

        foreach (var slot in slotsToAdd)
        {
            var openingHoursDetails = GetOpeningHoursDetails(openingHours, slot);
            if (openingHours.ClosedDates.Any(item => item == slot.Start.StartOfDay()))
            {
                slot.Available = false;
            }
            else if (openingHoursDetails.Closed)
            {
                slot.Available = false;
            }
            else if (openingHoursDetails.OpenAllDay)
            {
                slot.Available = true;
            }
            else
            {
                slot.Available = IsAvailable(TimeOnly.FromDateTime(slot.Start.DateTime), openingHoursDetails);
            }
        }

        if (slotsToAdd.Count > 0)
        {
            repositoryFactory.ResourceBookingSlotRepository.AddRange(slotsToAdd);
            savingNeeded = true;
        }

        var slotsToUpdate = existingResourceBookingSlots.Where(item => allSlotsDictionary.ContainsKey(item.Start)).ToList();
        foreach (var slot in slotsToUpdate)
        {
            var updateNeeded = false;
            var openingHoursDetails = GetOpeningHoursDetails(openingHours, slot);
            if (openingHours.ClosedDates.Any(item => item == slot.Start.StartOfDay()))
            {
                if (slot.Available)
                {
                    slot.Available = false;
                    updateNeeded = true;
                }
            }
            else if (openingHoursDetails.Closed)
            {
                if (slot.Available)
                {
                    slot.Available = false;
                    updateNeeded = true;
                }
            }
            else if (openingHoursDetails.OpenAllDay)
            {
                if (!slot.Available)
                {
                    slot.Available = true;
                    updateNeeded = true;
                }
            }
            else
            {
                var newValue = IsAvailable(TimeOnly.FromDateTime(slot.Start.DateTime), openingHoursDetails);

                if (slot.Available != newValue)
                {
                    slot.Available = newValue;
                    updateNeeded = true;
                }
            }

            if (updateNeeded)
            {
                repositoryFactory.ResourceBookingSlotRepository.UpdateRange(slotsToAdd);
                savingNeeded = true;
            }
        }

        if (savingNeeded)
        {
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    [Activity]
    public async Task ExecuteNextLocationResourcesSlotGenerationWorkflowAsync(string locationId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;

        await temporalClient.StartWorkflowAsync(
            (LocationResourceSlotGeneration workflow) =>
                workflow.ExecuteAsync(new LocationResourceSlotGenerationInput(locationId, timeProvider.GetUtcNow().AddDays(1))),
            new WorkflowOptions
            {
                Id = randomHelper.Generate(),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.RejectDuplicate,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });
    }

    private static OpeningHoursDetails GetOpeningHoursDetails(OpeningHours openingHours, ResourceBookingSlot slot) =>
        openingHours.DatesWithVariedOpeningHours.ContainsKey(slot.Start.StartOfDay())
            ? openingHours.DatesWithVariedOpeningHours[slot.Start.StartOfDay()]
            : slot.Start.DayOfWeek switch
            {
                DayOfWeek.Monday => openingHours.WeekOpeningHours.Monday,
                DayOfWeek.Tuesday => openingHours.WeekOpeningHours.Tuesday,
                DayOfWeek.Wednesday => openingHours.WeekOpeningHours.Wednesday,
                DayOfWeek.Thursday => openingHours.WeekOpeningHours.Thursday,
                DayOfWeek.Friday => openingHours.WeekOpeningHours.Friday,
                DayOfWeek.Saturday => openingHours.WeekOpeningHours.Saturday,
                DayOfWeek.Sunday => openingHours.WeekOpeningHours.Sunday,
                _ => throw new ArgumentOutOfRangeException()
            };

    private static bool IsAvailable(TimeOnly start, OpeningHoursDetails openingHoursDetails) =>
        start >= openingHoursDetails.From && start < openingHoursDetails.Until;
}
