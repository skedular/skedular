using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;

namespace Booking.Jobs.Jobs;

public class DeskRoomToLocationResourceSyncJob(
    IServiceProvider serviceProvider,
    ILogger<DeskRoomToLocationResourceSyncJob> logger) : BackgroundService
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
                        await repositoryFactory.OrganizationRepository.GetByIdAsync(location.Organization!.Id, true, true, true, cancellationToken);

                    ArgumentNullException.ThrowIfNull(organization);

                    var deskResourceType =
                        organization.ResourceTypes.SingleOrDefault(item => item.SystemType == OrganizationResourceTypeSystemTypeConstants.Desk);

                    if (deskResourceType is not null)
                    {
                        foreach (var desk in location.Desks)
                        {
                            var deskWithBookings = await repositoryFactory.DeskRepository.GetByIdAsync(desk.Id, true, cancellationToken);
                            ArgumentNullException.ThrowIfNull(deskWithBookings);

                            var existingResource = await repositoryFactory.LocationResourceRepository.GetByIdAsync(desk.Id, true, cancellationToken);
                            if (existingResource is null)
                            {
                                var locationResource = new LocationResource
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
                                    OrganizationResourceType = deskResourceType,
                                    Bookings = deskWithBookings.Bookings,
                                    PreferredByCustomers = deskWithBookings.PreferredByCustomers
                                };

                                repositoryFactory.LocationResourceRepository.Add(locationResource);
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
                                existingResource.OrganizationResourceType = deskResourceType;
                                existingResource.Bookings = existingResource.Bookings
                                    .Concat(deskWithBookings.Bookings.Where(item => existingResource.Bookings.All(booking => booking.Id != item.Id)))
                                    .ToList();

                                existingResource.PreferredByCustomers =
                                    existingResource.PreferredByCustomers
                                        .Concat(deskWithBookings.PreferredByCustomers.Where(item =>
                                            existingResource.PreferredByCustomers.All(preferredByCustomer => preferredByCustomer.Id != item.Id)))
                                        .ToList();

                                repositoryFactory.LocationResourceRepository.Update(existingResource);
                            }

                            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                        }
                    }

                    var roomResourceType =
                        organization.ResourceTypes.SingleOrDefault(item => item.SystemType == OrganizationResourceTypeSystemTypeConstants.Room);

                    if (roomResourceType is not null)
                    {
                        foreach (var room in location.Rooms)
                        {
                            var roomWithBookings = await repositoryFactory.RoomRepository.GetByIdAsync(room.Id, true, cancellationToken);
                            ArgumentNullException.ThrowIfNull(roomWithBookings);

                            var existingResource = await repositoryFactory.LocationResourceRepository.GetByIdAsync(room.Id, true, cancellationToken);
                            if (existingResource is null)
                            {
                                var locationResource = new LocationResource
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
                                    OrganizationResourceType = roomResourceType,
                                    Bookings = roomWithBookings.Bookings,
                                    PreferredByCustomers = roomWithBookings.PreferredByCustomers
                                };

                                repositoryFactory.LocationResourceRepository.Add(locationResource);
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
                                existingResource.OrganizationResourceType = roomResourceType;
                                existingResource.Bookings = existingResource.Bookings
                                    .Concat(roomWithBookings.Bookings.Where(item => existingResource.Bookings.All(booking => booking.Id != item.Id)))
                                    .ToList();

                                existingResource.PreferredByCustomers =
                                    existingResource.PreferredByCustomers
                                        .Concat(roomWithBookings.PreferredByCustomers.Where(item =>
                                            existingResource.PreferredByCustomers.All(preferredByCustomer => preferredByCustomer.Id != item.Id)))
                                        .ToList();

                                repositoryFactory.LocationResourceRepository.Update(existingResource);
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
                logger.LogError(ex, "Failed to run job: {job}", nameof(DeskRoomToLocationResourceSyncJob));
            }
        } while (true);
    }
}
