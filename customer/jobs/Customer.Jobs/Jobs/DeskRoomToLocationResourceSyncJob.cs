using Api.Shared.Services.Models;
using Customer.Shared.Database.Entities;
using Customer.Shared.Repositories;

namespace Customer.Jobs.Jobs;

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
                var locations = await repositoryFactory.LocationRepository.GetAllAsync(true, cancellationToken);

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
                                    Location = location,
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
                                existingResource.Location = location;
                                existingResource.PreferredByCustomers = existingResource.PreferredByCustomers;

                                repositoryFactory.LocationResourceRepository.Update(existingResource);
                            }

                            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                        }
                    }

                    var roomResourceType = organization.Tags.SingleOrDefault(item => item.Type == OrganizationTagTypeConstants.Room);
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
                                    Location = location,
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
                                existingResource.Location = location;
                                existingResource.PreferredByCustomers = existingResource.PreferredByCustomers;

                                repositoryFactory.LocationResourceRepository.Update(existingResource);
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
                logger.LogError(ex, "Failed to run job: {job}", nameof(DeskRoomToLocationResourceSyncJob));
            }
        } while (true);
    }
}
