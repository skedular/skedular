using Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Key;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Time;
using Event = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Type;

namespace Booking.Processors.Subscribers;

public class BookingInternalSubscriber(IRepositoryFactory repositoryFactory, IResourceBookingSlotHelperService resourceBookingSlotHelperService)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.GenerateResourceBookingSlot:
                await HandleGenerateResourceBookingSlotEventAsync(@event.ResourceId, cancellationToken);
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleGenerateResourceBookingSlotEventAsync(string resourceId, CancellationToken cancellationToken)
    {
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
            resourceBookingSlotHelperService.GetStartPeriod(),
            cancellationToken);

        var savingNeeded = false;
        var allSlots = resourceBookingSlotHelperService.CreateAllAvailableSlots(resource);
        var slotsToAdd = allSlots.Where(item => existingResourceBookingSlots.All(slot => slot.Start != item.Start)).ToList();

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

        var slotsToUpdate = existingResourceBookingSlots.Where(item => allSlots.Any(slot => slot.Start == item.Start)).ToList();
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
