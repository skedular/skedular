using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;

namespace Booking.Jobs.Jobs;

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
                var locations = await repositoryFactory.LocationRepository.GetAllAsync(true, true, true, true, cancellationToken);

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

                    var deskTag = organization.Tags.SingleOrDefault(item => item.Type == OrganizationTagTypeConstants.Desk);
                    if (deskTag is not null)
                    {
                        foreach (var desk in location.Desks)
                        {
                            var deskWithBookings = await repositoryFactory.DeskRepository.GetByIdAsync(desk.Id, true, cancellationToken);
                            ArgumentNullException.ThrowIfNull(deskWithBookings);

                            if (deskWithBookings.OrganizationTags.All(item => item.Id != deskTag.Id))
                            {
                                deskWithBookings.OrganizationTags = deskWithBookings.OrganizationTags.Concat([deskTag]).ToList();
                            }

                            var existingResource = await repositoryFactory.ResourceRepository.GetByIdAsync(desk.Id, true, cancellationToken);
                            if (existingResource is null)
                            {
                                var resource = new Resource
                                {
                                    Id = desk.Id,
                                    CreatedAt = desk.CreatedAt,
                                    ModifiedAt = desk.ModifiedAt,
                                    DeletedAt = desk.DeletedAt,
                                    EventRaisedAt = desk.EventRaisedAt,
                                    Name = desk.Name,
                                    Inactive = desk.Deactivated,
                                    RequireBookingApproval = desk.RequireBookingApproval,
                                    Color = desk.Color,
                                    Location = location,
                                    OrganizationTags = deskWithBookings.OrganizationTags,
                                    PreferredByCustomers = deskWithBookings.PreferredByCustomers
                                };

                                repositoryFactory.ResourceRepository.Add(resource);
                            }
                            else
                            {
                                existingResource.CreatedAt = desk.CreatedAt;
                                existingResource.ModifiedAt = desk.ModifiedAt;
                                existingResource.DeletedAt = desk.DeletedAt;
                                existingResource.EventRaisedAt = desk.EventRaisedAt;
                                existingResource.Name = desk.Name;
                                existingResource.Inactive = desk.Deactivated;
                                existingResource.RequireBookingApproval = desk.RequireBookingApproval;
                                existingResource.Color = desk.Color;
                                existingResource.Location = location;
                                existingResource.OrganizationTags = deskWithBookings.OrganizationTags;
                                existingResource.PreferredByCustomers = deskWithBookings.PreferredByCustomers;

                                repositoryFactory.ResourceRepository.Update(existingResource);
                            }

                            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                        }
                    }

                    var roomTag = organization.Tags.SingleOrDefault(item => item.Type == OrganizationTagTypeConstants.Room);
                    if (roomTag is not null)
                    {
                        foreach (var room in location.Rooms)
                        {
                            var roomWithBookings = await repositoryFactory.RoomRepository.GetByIdAsync(room.Id, true, cancellationToken);
                            ArgumentNullException.ThrowIfNull(roomWithBookings);

                            if (roomWithBookings.OrganizationTags.All(item => item.Id != roomTag.Id))
                            {
                                roomWithBookings.OrganizationTags = roomWithBookings.OrganizationTags.Concat([roomTag]).ToList();
                            }

                            var existingResource = await repositoryFactory.ResourceRepository.GetByIdAsync(room.Id, true, cancellationToken);
                            if (existingResource is null)
                            {
                                var resource = new Resource
                                {
                                    Id = room.Id,
                                    CreatedAt = room.CreatedAt,
                                    ModifiedAt = room.ModifiedAt,
                                    DeletedAt = room.DeletedAt,
                                    EventRaisedAt = room.EventRaisedAt,
                                    Name = room.Name,
                                    Inactive = room.Deactivated,
                                    RequireBookingApproval = room.RequireBookingApproval,
                                    Color = room.Color,
                                    Location = location,
                                    OrganizationTags = roomWithBookings.OrganizationTags,
                                    PreferredByCustomers = roomWithBookings.PreferredByCustomers
                                };

                                repositoryFactory.ResourceRepository.Add(resource);
                            }
                            else
                            {
                                existingResource.CreatedAt = room.CreatedAt;
                                existingResource.ModifiedAt = room.ModifiedAt;
                                existingResource.DeletedAt = room.DeletedAt;
                                existingResource.EventRaisedAt = room.EventRaisedAt;
                                existingResource.Name = room.Name;
                                existingResource.Inactive = room.Deactivated;
                                existingResource.RequireBookingApproval = room.RequireBookingApproval;
                                existingResource.Color = room.Color;
                                existingResource.Location = location;
                                existingResource.OrganizationTags = roomWithBookings.OrganizationTags;
                                existingResource.PreferredByCustomers = roomWithBookings.PreferredByCustomers;

                                repositoryFactory.ResourceRepository.Update(existingResource);
                            }

                            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
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
