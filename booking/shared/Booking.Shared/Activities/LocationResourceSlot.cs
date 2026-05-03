using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public record ExecuteAllLocationResourcesSlotGenerationWorkflowsResponse(bool ShallContinue, IReadOnlyList<string> ResourceIds);

public class LocationResourceSlot(
    IRepositoryFactory repositoryFactory,
    ILocationResourceBookingSlotsHelperService locationResourceBookingSlotsHelperService)
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
        foreach (var slotToUpdate in slotsToUpdate)
        {
            var updateNeeded = false;
            var openingHoursDetails = GetOpeningHoursDetails(openingHours, slotToUpdate);
            if (openingHours.ClosedDates.Any(item => item == slotToUpdate.Start.StartOfDay()))
            {
                if (slotToUpdate.Available)
                {
                    slotToUpdate.Available = false;
                    updateNeeded = true;
                }
            }
            else if (openingHoursDetails.Closed)
            {
                if (slotToUpdate.Available)
                {
                    slotToUpdate.Available = false;
                    updateNeeded = true;
                }
            }
            else if (openingHoursDetails.OpenAllDay)
            {
                if (!slotToUpdate.Available)
                {
                    slotToUpdate.Available = true;
                    updateNeeded = true;
                }
            }
            else
            {
                var newValue = IsAvailable(TimeOnly.FromDateTime(slotToUpdate.Start.DateTime), openingHoursDetails);

                if (slotToUpdate.Available != newValue)
                {
                    slotToUpdate.Available = newValue;
                    updateNeeded = true;
                }
            }

            if (updateNeeded)
            {
                repositoryFactory.ResourceBookingSlotRepository.Update(slotToUpdate);
                savingNeeded = true;
            }
        }

        if (savingNeeded)
        {
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
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
