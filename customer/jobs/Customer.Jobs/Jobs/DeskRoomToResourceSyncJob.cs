using Api.Shared.Services.Models;
using Customer.Shared.Repositories;

namespace Customer.Jobs.Jobs;

public class DeskRoomToResourceSyncJob(IServiceProvider serviceProvider, ILogger<DeskRoomToResourceSyncJob> logger) : BackgroundService
{
    private readonly string _jobName = typeof(DeskRoomToResourceSyncJob).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var locations = await repositoryFactory.LocationRepository.GetAllAsync(false, cancellationToken);

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

                    var existingResources = await repositoryFactory.ResourceRepository.GetAllAsync(location.Id, true, cancellationToken);
                    var deskTag = organization.Tags.SingleOrDefault(item => item.Type == OrganizationTagTypeConstants.Desk);
                    if (deskTag is not null)
                    {
                        foreach (var desk in location.Desks)
                        {
                            var deskDetails = await repositoryFactory.DeskRepository.GetByIdAsync(desk.Id, true, cancellationToken);
                            ArgumentNullException.ThrowIfNull(deskDetails);

                            var existingResource = existingResources.FirstOrDefault(item => item.Name == desk.Name);
                            if (existingResource is not null)
                            {
                                existingResource.CreatedAt = desk.CreatedAt;
                                existingResource.ModifiedAt = desk.ModifiedAt;
                                existingResource.DeletedAt = desk.DeletedAt;
                                existingResource.EventRaisedAt = desk.EventRaisedAt;
                                existingResource.Name = desk.Name;
                                existingResource.Location = location;
                                existingResource.PreferredByCustomers = deskDetails.PreferredByCustomers;

                                repositoryFactory.ResourceRepository.Update(existingResource);
                            }
                        }
                    }

                    var roomTag = organization.Tags.SingleOrDefault(item => item.Type == OrganizationTagTypeConstants.Room);
                    if (roomTag is not null)
                    {
                        foreach (var room in location.Rooms)
                        {
                            var roomDetails = await repositoryFactory.RoomRepository.GetByIdAsync(room.Id, true, cancellationToken);
                            ArgumentNullException.ThrowIfNull(roomDetails);

                            var existingResource = existingResources.FirstOrDefault(item => item.Name == room.Name);
                            if (existingResource is not null)
                            {
                                existingResource.CreatedAt = room.CreatedAt;
                                existingResource.ModifiedAt = room.ModifiedAt;
                                existingResource.DeletedAt = room.DeletedAt;
                                existingResource.EventRaisedAt = room.EventRaisedAt;
                                existingResource.Name = room.Name;
                                existingResource.Location = location;
                                existingResource.PreferredByCustomers = roomDetails.PreferredByCustomers;

                                repositoryFactory.ResourceRepository.Update(existingResource);
                            }
                        }
                    }
                }

                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

                logger.LogInformation("Finished running job: {job}", _jobName);

                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", _jobName);
            }
        } while (true);
    }
}
