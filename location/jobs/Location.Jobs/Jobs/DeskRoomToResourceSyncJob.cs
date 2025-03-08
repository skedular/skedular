using Api.Shared.Services.Models;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;

namespace Location.Jobs.Jobs;

public class DeskRoomToResourceSyncJob(
    IServiceProvider serviceProvider,
    ILogger<DeskRoomToResourceSyncJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var locations = await repositoryFactory.LocationRepository.GetAllAsync(true, true, true, cancellationToken);

                foreach (var location in locations)
                {
                    if (location.Organization is null)
                    {
                        continue;
                    }

                    ArgumentNullException.ThrowIfNull(location.Organization);

                    var organization =
                        await repositoryFactory.OrganizationRepository.GetByIdAsync(location.Organization!.Id, true, true, cancellationToken);

                    ArgumentNullException.ThrowIfNull(organization);

                    var deskResourceType = organization.Tags.SingleOrDefault(item => item.Type == OrganizationTagTypeConstants.Desk);
                    if (deskResourceType is not null)
                    {
                        foreach (var desk in location.Desks)
                        {
                            var deskWithBookings = await repositoryFactory.DeskRepository.GetByIdAsync(desk.Id, cancellationToken);
                            ArgumentNullException.ThrowIfNull(deskWithBookings);

                            if (deskWithBookings.OrganizationTags.All(item => item.Id != deskResourceType.Id))
                            {
                                deskWithBookings.OrganizationTags = deskWithBookings.OrganizationTags.Concat([deskResourceType]).ToList();
                            }

                            var existingResource = await repositoryFactory.ResourceRepository.GetByIdAsync(desk.Id, cancellationToken);
                            if (existingResource is null)
                            {
                                var resource = new Resource
                                {
                                    Id = desk.Id,
                                    CreatedAt = desk.CreatedAt,
                                    ModifiedAt = desk.ModifiedAt,
                                    DeletedAt = desk.DeletedAt,
                                    Name = desk.Name,
                                    Inactive = desk.Deactivated,
                                    RequireBookingApproval = desk.RequireBookingApproval,
                                    Color = desk.Color,
                                    Location = location,
                                    OrganizationTags = deskWithBookings.OrganizationTags
                                };

                                repositoryFactory.ResourceRepository.Add(resource);
                            }
                            else
                            {
                                existingResource.CreatedAt = desk.CreatedAt;
                                existingResource.ModifiedAt = desk.ModifiedAt;
                                existingResource.DeletedAt = desk.DeletedAt;
                                existingResource.Name = desk.Name;
                                existingResource.Inactive = desk.Deactivated;
                                existingResource.RequireBookingApproval = desk.RequireBookingApproval;
                                existingResource.Color = desk.Color;
                                existingResource.Location = location;
                                existingResource.OrganizationTags = deskWithBookings.OrganizationTags;

                                repositoryFactory.ResourceRepository.Update(existingResource);
                            }

                            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                        }
                    }

                    var roomResourceType = organization.Tags.SingleOrDefault(item => item.Type == OrganizationTagTypeConstants.Room);
                    if (roomResourceType is not null)
                    {
                        foreach (var room in location.Rooms)
                        {
                            var roomWithBookings = await repositoryFactory.RoomRepository.GetByIdAsync(room.Id, cancellationToken);
                            ArgumentNullException.ThrowIfNull(roomWithBookings);

                            if (roomWithBookings.OrganizationTags.All(item => item.Id != roomResourceType.Id))
                            {
                                roomWithBookings.OrganizationTags = roomWithBookings.OrganizationTags.Concat([roomResourceType]).ToList();
                            }

                            var existingResource = await repositoryFactory.ResourceRepository.GetByIdAsync(roomWithBookings.Id, cancellationToken);
                            if (existingResource is null)
                            {
                                var resource = new Resource
                                {
                                    Id = room.Id,
                                    CreatedAt = room.CreatedAt,
                                    ModifiedAt = room.ModifiedAt,
                                    DeletedAt = room.DeletedAt,
                                    Name = room.Name,
                                    Inactive = room.Deactivated,
                                    RequireBookingApproval = room.RequireBookingApproval,
                                    Color = room.Color,
                                    Location = location,
                                    OrganizationTags = roomWithBookings.OrganizationTags
                                };

                                repositoryFactory.ResourceRepository.Add(resource);
                            }
                            else
                            {
                                existingResource.CreatedAt = room.CreatedAt;
                                existingResource.ModifiedAt = room.ModifiedAt;
                                existingResource.DeletedAt = room.DeletedAt;
                                existingResource.Name = room.Name;
                                existingResource.Inactive = room.Deactivated;
                                existingResource.RequireBookingApproval = room.RequireBookingApproval;
                                existingResource.Color = room.Color;
                                existingResource.Location = location;
                                existingResource.OrganizationTags = roomWithBookings.OrganizationTags;

                                repositoryFactory.ResourceRepository.Update(existingResource);
                            }

                            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(DeskRoomToResourceSyncJob));
            }
        } while (true);
    }
}
