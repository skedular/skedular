using Api.Shared.Services.Models;
using Enterprise.Shared.Random;
using Organization.Shared.Database.Entities;
using Organization.Shared.Repositories;

namespace Organization.Jobs.Jobs;

public class BuiltInTagsSyncJob(IServiceProvider serviceProvider, ILogger<BuiltInTagsSyncJob> logger, IRandomHelper randomHelper) : BackgroundService
{
    private readonly string _jobName = typeof(BuiltInTagsSyncJob).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var organizations = await repositoryFactory.OrganizationRepository.GetAllAsync(cancellationToken);

                foreach (var organization in organizations)
                {
                    if (organization.Tags.All(item => item.Type != OrganizationTagTypeConstants.ResourceDesk))
                    {
                        repositoryFactory.TagRepository.Add(
                            new Tag
                            {
                                Id = randomHelper.Generate(),
                                Name = "Desk",
                                Type = OrganizationTagTypeConstants.ResourceDesk,
                                Color = null,
                                Organization = organization
                            });
                    }

                    if (organization.Tags.All(item => item.Type != OrganizationTagTypeConstants.ResourceRoom))
                    {
                        repositoryFactory.TagRepository.Add(
                            new Tag
                            {
                                Id = randomHelper.Generate(),
                                Name = "Room",
                                Type = OrganizationTagTypeConstants.ResourceRoom,
                                Color = null,
                                Organization = organization
                            });
                    }

                    if (organization.Tags.All(item => item.Type != OrganizationTagTypeConstants.ResourceParking))
                    {
                        repositoryFactory.TagRepository.Add(
                            new Tag
                            {
                                Id = randomHelper.Generate(),
                                Name = "Parking",
                                Type = OrganizationTagTypeConstants.ResourceParking,
                                Color = null,
                                Organization = organization
                            });
                    }

                    if (organization.Tags.All(item => item.Type != OrganizationTagTypeConstants.ResourceOthers))
                    {
                        repositoryFactory.TagRepository.Add(
                            new Tag
                            {
                                Id = randomHelper.Generate(),
                                Name = "Others",
                                Type = OrganizationTagTypeConstants.ResourceOthers,
                                Color = null,
                                Organization = organization
                            });
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
