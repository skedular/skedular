using Api.Shared.Services.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;

namespace Booking.Jobs.Jobs;

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
                var transactionBuilder = scope.ServiceProvider.GetRequiredService<IDbTransactionBuilder>();
                var locations = await repositoryFactory.LocationRepository.GetAllAsync(false, false, false, false, cancellationToken);

                await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

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

                            if (deskDetails.OrganizationTags.All(item => item.Id != deskTag.Id))
                            {
                                deskDetails.OrganizationTags = deskDetails.OrganizationTags.Concat([deskTag]).ToList();
                            }

                            var existingResource = existingResources.FirstOrDefault(item => item.Name == desk.Name);
                            if (existingResource is not null)
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
                                existingResource.OrganizationTags = deskDetails.OrganizationTags;
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

                            if (roomDetails.OrganizationTags.All(item => item.Id != roomTag.Id))
                            {
                                roomDetails.OrganizationTags = roomDetails.OrganizationTags.Concat([roomTag]).ToList();
                            }

                            var existingResource = existingResources.FirstOrDefault(item => item.Name == room.Name);
                            if (existingResource is not null)
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
                                existingResource.OrganizationTags = roomDetails.OrganizationTags;
                                existingResource.PreferredByCustomers = roomDetails.PreferredByCustomers;

                                repositoryFactory.ResourceRepository.Update(existingResource);
                            }
                        }
                    }
                }

                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

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
