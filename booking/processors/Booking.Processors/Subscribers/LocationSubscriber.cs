using Api.Shared.Clients.Events.Skedular.Location.V1;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using Constants = Api.Shared.Services.Constants;
using IMapper = Booking.Processors.Mappers.IMapper;
using Location = Booking.Shared.Database.Entities.Location;
using Type = Api.Shared.Clients.Events.Skedular.Location.V1.Type;

namespace Booking.Processors.Subscribers;

public class LocationSubscriber(
    ILogger<LocationSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    IDbTransactionBuilder transactionBuilder,
    ITemporalService temporalService) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.LocationUpserted:
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(@event.Data.Location.OrganizationId);

                    var location = mapper.MapTo(@event);
                    var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(location.Organization!.Id, cancellationToken);
                    var existingLocation = await repositoryFactory.LocationRepository.UpsertNakedAsync(location.Id, organization, cancellationToken);
                    if (existingLocation.EventRaisedAt > location.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Location event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
                    await HandleLocationUpsertedEventAsync(location, existingLocation, organization, cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                }
                break;

            case Type.LocationDeleted:
                {
                    var location = mapper.MapTo(@event);
                    var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, true, cancellationToken);
                    if (existingLocation is not null && existingLocation.EventRaisedAt > location.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Location event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    if (existingLocation is null)
                    {
                        return EventSubscriberResults.Success;
                    }

                    await HandleLocationDeletedEventAsync(existingLocation, cancellationToken);
                }
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleLocationUpsertedEventAsync(
        Shared.Models.Location location,
        Location existingLocation,
        Organization organization,
        CancellationToken cancellationToken)
    {
        var organizationTags = new List<OrganizationTag>();
        var organizationTagIds = location.OrganizationTags.Select(item => item.Id).Distinct().ToList();
        foreach (var tagId in organizationTagIds)
        {
            organizationTags.Add(await repositoryFactory.OrganizationTagRepository.UpsertNakedAsync(tagId, organization, cancellationToken));
        }

        var locationOpeningHoursChanged = !location.OpeningHours.IsEqual(existingLocation.OpeningHours);
        existingLocation =
            repositoryFactory.LocationRepository.Update(mapper.MergeToEntity(location, existingLocation, organization, organizationTags));

        (existingLocation, var resourceIdsToRegenerateBookingSlots) =
            await RebuildResourcesAsync(location, existingLocation, organization, cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        if (locationOpeningHoursChanged)
        {
            if (existingLocation.Organization?.CustomDomain != Constants.SkedularPublicLocationsCustomDomainName)
            {
                // Regenerate all
                await temporalService.StartWorkflowGenerateLocationResourcesSlotsAsync(
                    new GenerateLocationResourcesSlotsInput(existingLocation.Id, null),
                    cancellationToken);
            }
        }
        else
        {
            if (existingLocation.Organization?.CustomDomain != Constants.SkedularPublicLocationsCustomDomainName &&
                resourceIdsToRegenerateBookingSlots.Count != 0)
            {
                // Regenerate those changed
                await temporalService.StartWorkflowGenerateResourcesSlotsAsync(
                    existingLocation.Id,
                    new GenerateResourcesSlotsInput(resourceIdsToRegenerateBookingSlots),
                    cancellationToken);
            }
        }
    }

    private async Task HandleLocationDeletedEventAsync(Location existingLocation, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.LocationRepository.Remove(existingLocation);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<(Location, ICollection<string>)> RebuildResourcesAsync(
        Shared.Models.Location location,
        Location existingLocation,
        Organization? organization,
        CancellationToken cancellationToken)
    {
        if (organization is null)
        {
            return (existingLocation, []);
        }

        var organizationTags = new List<OrganizationTag>();
        var organizationTagIds = location.Resources.SelectMany(item => item.OrganizationTags.Select(tag => tag.Id)).Distinct().ToList();
        foreach (var tagId in organizationTagIds)
        {
            organizationTags.Add(await repositoryFactory.OrganizationTagRepository.UpsertNakedAsync(tagId, organization, cancellationToken));
        }

        var resourceIdsToRegenerateBookingSlots = new List<string>();
        var resources = await repositoryFactory.ResourceRepository.GetByLocationIdAsync(existingLocation.Id, cancellationToken);
        var itemsToRemove = resources.Where(resource => location.Resources.All(item => item.Id != resource.Id)).ToList();
        var updatedItems = resources
            .Where(resource => location.Resources.Any(item => item.Id == resource.Id))
            .Select(existingResource =>
            {
                var filteredOrganizationTags = organizationTags
                    .Where(tag => location.Resources
                        .First(item => item.Id == existingResource.Id).OrganizationTags
                        .Any(organizationTag => organizationTag.Id == tag.Id))
                    .ToList();


                var resource = location.Resources.First(item => item.Id == existingResource.Id);

                if (resource.IsAvailableHoursOverridden != existingResource.IsAvailableHoursOverridden ||
                    !resource.AvailableHours.IsEqual(existingResource.AvailableHours))
                {
                    resourceIdsToRegenerateBookingSlots.Add(resource.Id);
                }

                var updatedResource = mapper.MergeToEntity(resource, existingResource, existingLocation, filteredOrganizationTags);
                updatedResource.DeletedAt = null;

                return repositoryFactory.ResourceRepository.Update(updatedResource);
            })
            .ToList();
        var addedItems = location.Resources
            .Where(resource => resources.All(item => item.Id != resource.Id))
            .Select(resource =>
            {
                var filteredOrganizationTags = organizationTags
                    .Where(tag => resource.OrganizationTags.Any(organizationTag => organizationTag.Id == tag.Id))
                    .ToList();

                return repositoryFactory.ResourceRepository.Add(mapper.MapToEntity(resource, existingLocation, filteredOrganizationTags));
            })
            .ToList();

        repositoryFactory.ResourceRepository.RemoveRange(itemsToRemove);
        existingLocation.Resources = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        resourceIdsToRegenerateBookingSlots.AddRange(addedItems.Select(item => item.Id));

        return (existingLocation, resourceIdsToRegenerateBookingSlots.Distinct().ToList());
    }
}
